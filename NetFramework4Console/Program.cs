using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetFramework4Console
{
    class Program
    {
        private const string PoolName = "FirehoseLogger";
        static void Main(string[] args)
        {
            initLoopingTasks();
            initPoolObserver();
            Console.WriteLine("done");
            Console.ReadLine();
        }

        private static void initLoopingTasks()
        {
            Random random = new Random();
            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 10000; i++)
                {
                    var loopvariable = i;
                    AsyncTasks.Run(() =>
                    {
                       Console.WriteLine("loopvariable: \t" + loopvariable);
                        //var randomDelay = random.Next(1000, 3000);
                        //Delay(randomDelay).ContinueWith(t =>
                        //{
                           
                        //});

                    }, PoolName);
                }
            });
        }
        private static void initPoolObserver()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                      Delay(1000).ContinueWith(t =>
                       {
                           try
                           {
                               var taskPool = AsyncTasks.GetPool(PoolName);
                               Console.WriteLine("taskPool.GetQueueSize():\t " + taskPool.GetQueueSize());
                           }
                           catch
                           { 
                           }
                       }).Wait();
                };
            });
        }
        static Task Delay(int milliseconds)
        {
            var tcs = new TaskCompletionSource<object>();
            new Timer(_ => tcs.SetResult(null)).Change(milliseconds, -1);
            return tcs.Task;
        }
    }
}
