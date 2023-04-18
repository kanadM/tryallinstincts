using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace NetFramework4Console
{
    public class TaskPool : ITaskPool
    {
        public readonly BlockingCollection<Action> _queue = null;
        private Task[] _tasks;

        public int GetQueueSize()
        {
            return _queue.Count;
        }
        public TaskPool(int size)
        {
            _queue = new BlockingCollection<Action>(new ConcurrentQueue<Action>());
            _tasks = new Task[size];
            for (int i = 0; i < size; i++)
            {
                _tasks[i] = Task.Factory.StartNew(() =>
                {
                    //foreach (var work in _queue.GetConsumingEnumerable())
                    //{
                    //    try
                    //    {
                    //        work();
                    //    }
                    //    catch { }
                    //}

                    do
                    {
                        if (_queue.TryTake(out var work, 5000))
                        {
                            try
                            {
                                Thread.Sleep(1);
                                work();
                            }
                            catch { }
                        }
                        else
                        {
                            Console.WriteLine("no item for last 5sec");
                            Thread.Sleep(5000);
                        }
                    } while (true);
                }, TaskCreationOptions.LongRunning);
            }
        }


        public void Enqueue(Action action)
        {
            _queue.Add(action);
        }

        public void StopAdding()
        {
            _queue.CompleteAdding();
        }
    }
}
