using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Collections
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var originalRecords = new List<Record>();
            originalRecords.Add(1, 100);
            var securityMasterInfo = new List<Record>();
            securityMasterInfo.Add(100,1);

            Stopwatch sw = Stopwatch.StartNew();
            Dictionary<int, Record> dict = securityMasterInfo.ToDictionary(s => s.Key, s=>s);
            foreach (var item in originalRecords)
            {
                item.Value = dict[item.Key];
            }
            sw.Stop();
            Console.WriteLine($"time taken for mapping {sw.ElapsedMilliseconds}");
            foreach (var item in originalRecords)
            {
             //   Console.WriteLine($"{item.Key} - {item.Value.Key}");
            }
            Console.ReadLine();
        }
    }

    public class Record
    {
        public int Key { get; set; }
        public Record Value { get; set; }

    }

    public static class Extention
    {
        public static void Add(this List<Record> records, int start, int end)
        {
            if (start < end)
                for (int i = start; i <= end; i++)
                {
                    records.Add(new Record { Key = i });
                }
            else
                for (int i = start; i >= end; i--)
                {
                    records.Add(new Record { Key = i });
                }
        }
    }
}
