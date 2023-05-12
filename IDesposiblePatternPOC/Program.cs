using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace IDesposiblePatternPOC
{
    class DisposableCollection<T> : IEnumerable<T>, IDisposable where T : IDisposable
    {
        private bool disposedValue;
        public DisposableCollection(IEnumerable<T> records)
        {
            _records = records;
        }
        IEnumerable<T> _records;
        public IEnumerator<T> GetEnumerator()
        {
            return _records.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _records.GetEnumerator();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var item in _records)
                    {
                        item.Dispose();
                    }
                    // TODO: dispose managed state (managed objects)
                }
                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }
          
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    class Program
    { 
        static void Main(string[] args)
        {
            
            int k = 0;
            using (var producers = new DisposableCollection<KafkaProducer>(GetProducers()))
            //using (var producers = new DisposableCollection<KafkaProducer>(GetProducers()))
            using (var tt = new TimeTracker(nameof(Main)))
            {
                {
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            k++; 
                            Parallel.ForEach(producers,(producer)=>{ producer.Produce($"i:{i} j:{j}"); });
                        }
                    }
                }
            }

        }
        private static IEnumerable<KafkaProducer> GetProducers()
        {
            //var lst = new List<KafkaProducer> { new KafkaProducer(), new KafkaProducer() };
            //foreach (var item in lst)
            {
                yield return getKafkaProducer();
                yield return getKafkaProducer();
            }
        }
        private static KafkaProducer getKafkaProducer()
        {
            return new KafkaProducer();
        }
    }
    public class TimeTracker : IDisposable
    {
        public readonly Stopwatch _watch;
        private readonly string _name;

        public TimeTracker(string name)
        {
            _name = name;
            _watch = new Stopwatch();
            _watch.Start();
        }

        public void Dispose()
        {
            var timeTakenInMs = _watch.ElapsedMilliseconds;
            Console.WriteLine($"time taken by method {_name} is {_watch.ElapsedMilliseconds} ms");

        }
    }

    class KafkaProducer : IDisposable
    {
        private bool disposedValue;
        public void Produce(string message)
        {
            Console.WriteLine(message);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Console.WriteLine("on line B");
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                Console.WriteLine("on line C");
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Console.WriteLine("on line A");
            Dispose(disposing: true);
            Console.WriteLine("on line D");
            GC.SuppressFinalize(this);
        }
    }
}
