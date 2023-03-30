using System;
using StandardAnalyzer = Lucene.Net.Analysis.Standard.StandardAnalyzer;
using Document = Lucene.Net.Documents.Document;
using Field = Lucene.Net.Documents.Field;
using Analyzer = Lucene.Net.Analysis.Analyzer;
using QueryParser = Lucene.Net.QueryParsers.QueryParser;
using DateTools = Lucene.Net.Documents.DateTools;
using IndexWriter = Lucene.Net.Index.IndexWriter;
using IndexSearcher = Lucene.Net.Search.IndexSearcher;
using Query = Lucene.Net.Search.Query;
using Searcher = Lucene.Net.Search.Searcher;
using Lucene.Net.Search;

namespace ApacheLucene1
{
    class Program
    {
        internal static readonly System.IO.FileInfo INDEX_DIR =
            new System.IO.FileInfo($"index_{DateTime.Now.Ticks}");
        [STAThread]
        public static void Main(System.String[] args)
         {
            System.String usage = typeof(Program) + " <root_directory>";
            if (args.Length == 0)
            {
                System.Console.Error.WriteLine("Usage: " + usage);
                System.Environment.Exit(1);
            }

            // Check whether the "index" directory exists.
            // If not, create it; otherwise, exit program.
            bool tmpBool = System.IO.Directory.Exists(INDEX_DIR.FullName);
            if (tmpBool)
            {
                System.Console.Out.WriteLine("Cannot save index to '" +
                    INDEX_DIR + "' directory, please delete it first");
                System.Environment.Exit(1);
            }

            System.IO.FileInfo docDir = new System.IO.FileInfo(args[0]);
            tmpBool = System.IO.Directory.Exists(docDir.FullName);
            if (!tmpBool)
            {
                System.Console.Out.WriteLine("Document directory '" +
                    docDir.FullName + "' does not exist or is not readable, " +
                    "please check the path");
                System.Environment.Exit(1);
            }

            System.DateTime start = System.DateTime.Now;
            try
            {
                IndexWriter writer =
                    new IndexWriter(Lucene.Net.Store.FSDirectory.Open(INDEX_DIR.FullName), new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_CURRENT), true, Lucene.Net.Index.IndexWriter.MaxFieldLength.LIMITED);
                System.Console.Out.WriteLine("Indexing to directory '" +
                                             INDEX_DIR + "'...");
                IndexDocs(writer, docDir);
                System.Console.Out.WriteLine("Optimizing...");
                writer.Optimize();
                writer.Close();

                System.DateTime end = System.DateTime.Now;
                System.Console.Out.WriteLine(end.Ticks - start.Ticks +
                                             " total milliseconds");
            }
            catch (System.IO.IOException e)
            {
                System.Console.Out.WriteLine(" caught a " + e.GetType() +
                                             "\n with message: " + e.Message);
            }

            new SearchFiles().Main(null);
        }

        public static void IndexDocs(IndexWriter writer,
                                      System.IO.FileInfo file)
        {
            if (System.IO.Directory.Exists(file.FullName))
            {
                System.String[] files =
                    System.IO.Directory.GetFileSystemEntries(file.FullName);
                if (files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        IndexDocs(writer, new System.IO.FileInfo(files[i]));
                    }
                }
            }
            else
            {
                System.Console.Out.WriteLine("adding " + file);
                writer.AddDocument(IndexDocument(file));
            }
        }

        public static Document IndexDocument(System.IO.FileInfo f)
        {
            // Make a new, empty document
            Document doc = new Document();

            // Add the path of the file as a field named "path".
            // Use a field that is indexed (i.e., searchable), but don't
            // tokenize the field into words.
            doc.Add(new Field("path", f.FullName, Field.Store.YES,
                              Field.Index.ANALYZED));

            // Add the fileName as a field named "fileName".
            // Use a field that is indexed (i.e., searchable), but don't
            // tokenize the field into words.
            doc.Add(new Field("fileName", f.Name, Field.Store.YES,
                              Field.Index.ANALYZED));


            // Add the last modified date of the file to a field named
            // "modified". Use a field that is indexed (i.e., searchable),
            // but don't tokenize the field into words.
            doc.Add(new Field("modified",
                              DateTools.DateToString(f.LastWriteTime,
                              DateTools.Resolution.MINUTE),
                              Field.Store.YES, Field.Index.ANALYZED));

            // Add the contents of the file to a field named "contents".
            // Specify a Reader, so that the text of the file is tokenized
            // and indexed, but not stored. Note that FileReader expects
            // the file to be in the system's default encoding. If that's
            // not the case, searching for special characters will fail.
            doc.Add(new Field("contents",
                              new System.IO.StreamReader(f.FullName,
                              System.Text.Encoding.Default)));

            // Return the document
            return doc;
        }
    }

    class SearchFiles
    {
        [STAThread]
        public void Main(System.String[] args)
        {
            try
            {
                     
                Searcher searcher = new IndexSearcher(Lucene.Net.Store.FSDirectory.Open(Program.INDEX_DIR.FullName));
                Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_CURRENT);


                // Create a new StreamReader using standard input as the stream
                System.IO.StreamReader streamReader =
                    new System.IO.StreamReader(
                        // Sets reader's input stream to the standard input stream
                        new System.IO.StreamReader(
                            System.Console.OpenStandardInput(),
                            System.Text.Encoding.Default).BaseStream,
                        // Sets reader's encoding to whatever standard input is using
                        new System.IO.StreamReader(
                            System.Console.OpenStandardInput(),
                            System.Text.Encoding.Default).CurrentEncoding);
                while (true)
                {
                    System.Console.Out.Write("Query: ");
                    System.String line = streamReader.ReadLine();

                    if (line.Length <= 0)
                        break;

                    var  QueryParser  = new Lucene.Net.QueryParsers.MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_CURRENT, new [] { "path", "contents", "fileName" } , analyzer);
                    Query query = QueryParser.Parse(line);
                    System.Console.Out.WriteLine("Searching for: " +
                                                 query.ToString("contents"));

                    TopDocs hits = searcher.Search(query,10000);
                    System.Console.Out.WriteLine(hits.TotalHits +
                                                 " total matching documents");

                    int HITS_PER_PAGE = 10;
                    for (int start = 0; start < hits.TotalHits; start += HITS_PER_PAGE)
                    {
                        int end = System.Math.Min(hits.TotalHits, start + HITS_PER_PAGE);
                        for (int i = start; i < end; i++)
                        {
                            var doc =searcher.Doc(hits.ScoreDocs[i].Doc);
                            System.String path = doc.Get("path");
                            if (path != null)
                            {
                                System.Console.Out.WriteLine(i + ". " + path);
                            }
                            else
                            {
                                System.String url = doc.Get("url");
                                if (url != null)
                                {
                                    System.Console.Out.WriteLine(i + ". " + url);
                                    System.Console.Out.WriteLine("   - " + doc.Get("title"));
                                }
                                else
                                {
                                    System.Console.Out.WriteLine(i + ". " +
                                                       "No path nor URL for this document");
                                }
                            }
                        }

                        if (hits.TotalHits > end)
                        {
                            System.Console.Out.Write("more (y/n) ? ");
                            line = streamReader.ReadLine();
                            if (line.Length <= 0 || line[0] == 'n')
                                break;
                        }
                    }
                }
                searcher.Close();
            }
            catch (System.Exception e)
            {
                System.Console.Out.WriteLine(" caught a " + e.GetType() +
                                             "\n with message: " + e.Message);
            }
        }
    }
}