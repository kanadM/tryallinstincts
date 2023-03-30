//using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static net4App.Program;

namespace net4App
{
    public class BatchingLogicCheck
    {
        public static void Execute()
        {

            var list = new List<PortfolioSecurity1>();
            for (int i = 0; i < 1020500; i++)
            {
                list.Add(new PortfolioSecurity1
                {
                    PortfolioSecurityId = 8502 + i,
                    PortfolioId = 9999,
                    SecurityId = 8502 + i,
                    IdentifierDisplay = "asdf",
                    OrderIndex = null,
                    SecurityNameDisplay = "asdf asdfasddfwe",
                });
            }

            int batchSize = 20000;
            using (new Profiler("BatchingLogicCheck_Chunkify"))
            {
                var baches = list.Chunkify(batchSize);
                foreach (var batch in baches)
                {
                    var lst = batch.ToList();
                    //Console.WriteLine($"{batch.Count()}");
                }
            }
            using (new Profiler("BatchingLogicCheck_Split"))
            {
                var baches = list.Split(batchSize);
                foreach (var batch in baches)
                {
                    var lst = batch.ToList();
                    //Console.WriteLine($"{batch.Count()}");
                }
            }
            using (new Profiler("BatchingLogicCheck_ToChunks"))
            {
                var baches = list.ToChunks(batchSize);
                foreach (var batch in baches)
                {
                    var lst = batch.ToList();
                    //Console.WriteLine($"{batch.Count()}");
                }
            }
            //using (new Profiler("BatchingLogicCheck_MoreLinq"))
            //{
            //    var baches = list.MoreLinq(batchSize);
            //    foreach (var batch in baches)
            //    {
            //        var lst = batch.ToList();
            //        //Console.WriteLine($"{batch.Count()}");
            //    }
            //}
            using (new Profiler("BatchingLogicCheck_simpleBatching"))
            {
                var baches = list.Batch(batchSize);
                foreach (var batch in baches)
                {
                    var lst = batch.ToList();
                    //Console.WriteLine($"{batch.Count()}");
                }
            }

        } 
    }

    public static class SummaryHelper
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> entities, int batchSize, bool mergeLeftRecordsToLastEntry = true)
        {
            var chunkedList = new List<List<T>>();
            var entitiesCount = entities.Count();
            if (entitiesCount < batchSize)
            {
                chunkedList.Add(entities.ToList());
                return chunkedList;
            }
            var temporary = new List<T>();
            int i = 0;
            for (; i < entitiesCount; i++)
            {
                var e = entities.ElementAt(i);
                temporary.Add(e);
                if (temporary.Count() == batchSize)
                {
                    chunkedList.Add(temporary);
                    temporary = new List<T>();
                }
            }
            if (i == entitiesCount && temporary.Count() > 0) //reached end
            {
                if (chunkedList.Count() > 0 && mergeLeftRecordsToLastEntry)
                    chunkedList.Last().AddRange(temporary);
                else
                    chunkedList.Add(temporary);
            }
            return chunkedList;
        }

        public static IEnumerable<IEnumerable<T>> Chunkify<T>(this IEnumerable<T> source, int size)
        {
            int count = 0;
            using (var iter = source.GetEnumerator())
            {
                while (iter.MoveNext())
                {
                    var chunk = new T[size];
                    count = 1;
                    chunk[0] = iter.Current;
                    for (int i = 1; i < size && iter.MoveNext(); i++)
                    {
                        chunk[i] = iter.Current;
                        count++;
                    }
                    if (count < size)
                    {
                        Array.Resize(ref chunk, count);
                    }
                    yield return chunk;
                }
            }
        }

        public static IEnumerable<IEnumerable<T>> ToChunks<T>(this IEnumerable<T> values, int chunkSize)
        {
            using (var enumerator = values.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return getChunk(enumerator, chunkSize);
                }
            }
        }
        private static IEnumerable<T> getChunk<T>(IEnumerator<T> enumerator, int chunkSize)
        {
            do
            {
                yield return enumerator.Current;
            } while (--chunkSize > 0 && enumerator.MoveNext());
        }

        //public static IEnumerable<IEnumerable<T>> MoreLinq<T>(this IEnumerable<T> source, int size)
        //{
        //    return source.Batch(size);
        //}
        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(
                  this IEnumerable<TSource> source, int size)
        {
            TSource[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new TSource[size];

                bucket[count++] = item;
                if (count != size)
                    continue;

                yield return bucket;

                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0)
                yield return bucket.Take(count).ToArray();
        }
    }
}
