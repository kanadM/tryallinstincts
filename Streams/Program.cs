using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Streams
{
    class Program
    {
        static void Main(string[] args)
        { 
            //fileStream();
            bufferStream();
            Console.ReadKey();
        }

        static void fileStream()
        {
            String path = @"D:\Example.txt";
            using (StreamReader sr = File.OpenText(path))
            {
                String s = "";

                while ((s = sr.ReadLine()) != null)
                {
                    Console.WriteLine(s);
                }
            }
        }
        static void bufferStream()
        {
            Stopwatch watch = Stopwatch.StartNew();
            using (FileStream sw = new FileStream("D:\\MyTextFile.txt",FileMode.Create))
            {
                for (int i = 0; i < 50000; i++)
                {
                    byte[] bytes = Encoding.ASCII.GetBytes("This is a sample text.");
                    sw.Write("This is a sample text.",);
                }
            }
            Console.WriteLine($"W/O Buffer {watch.ElapsedMilliseconds}");
            watch.Restart();
            using (FileStream fileStream = new FileStream("D:\\MyTextFile.txt", FileMode.Create, FileAccess.ReadWrite))
            {
                using (BufferedStream bufferedStream = new BufferedStream(fileStream, 1024))
                {
                    for (int i = 0; i < 50000; i++)
                    {
                        byte[] bytes = Encoding.ASCII.GetBytes("This is a sample text.");
                        bufferedStream.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            Console.WriteLine($"with Buffer {watch.ElapsedMilliseconds}");
        }
        //static void GZipCompress()
        //{
        //    string fileToCompress = "D:\\MyTextFile.txt";
        //    byte[] data = null;
        //    byte[] compressed = null; 
        //    using (StreamReader sr = File.OpenText(fileToCompress))
        //    {
        //        data = Encoding.ASCII.GetBytes(sr.ReadToEnd());
        //    }
        //    using (Stream zipStream = new MemoryStream())
        //    {
        //        using (var zip = new System.IO.Compression.GZipStream(zipStream,
        //            System.IO.Compression.CompressionMode.Compress, true))
        //        {
        //            zip.Write(data, 0, data.Length);
        //        }

        //        zipStream.Seek(0, SeekOrigin.Begin);
        //        compressed = new byte[zipStream.Length];
        //        zipStream.Read(compressed, 0, compressed.Length);
        //    }
        //    File.WriteAllBytes("D:\\MyTextFile.compressed", compressed);
        //}
        //static void GZipDecompress()
        //{
        //    string fileToCompress = "D:\\MyTextFile.compressed";
        //    byte[] data = null;
        //    byte[] decompressed = null;
        //    using (StreamReader sr = File.OpenText(fileToCompress))
        //    {
        //        data = Encoding.ASCII.GetBytes(sr.ReadToEnd());
        //    }
        //    decompressed = new byte[BitConverter.ToInt32(data, 0)];
        //    using (System.IO.MemoryStream memStream = new System.IO.MemoryStream())
        //    {
        //        //Remove size from the compressed data
        //        memStream.Write(data, 4, data.Length - 4);
        //        memStream.Seek(0, System.IO.SeekOrigin.Begin);

        //        using (var zip = new System.IO.Compression.GZipStream(memStream,
        //            System.IO.Compression.CompressionMode.Decompress, true))
        //        {
        //            zip.Read(decompressed, 0, data.Length);
        //        }


        //    }
        //    File.WriteAllBytes("D:\\MyTextFile.Decompress", decompressed);


        //}
    }
}
