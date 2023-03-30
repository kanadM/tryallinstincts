using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExAsyncLocal
{
    class Program
    {
        //Control
        //private static string _staticField;

        //[ThreadStatic]
        //private static string _staticField;
        //private static ThreadLocal<string> _staticField = new ThreadLocal<string>();
        private static AsyncLocal<string> _staticField = new AsyncLocal<string>();
        static async Task Main(string[] args)
        {
            List<int> lst = new List<int> { 0, 1, 2, 3, 4 };
            List<Task> tasks = new List<Task>();

            foreach (var item in lst)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var threadId = Thread.CurrentThread.ManagedThreadId;
                    _staticField.Value = $"Task:[{item}] This is data from thread {threadId}";
                    await Task.Delay(200);
                    Console.WriteLine($"Task:[{item}]; Thread: {threadId}; Value: {_staticField.Value}");
                }));
            }

            await Task.WhenAll(tasks);
            
            
            
            
            //Parallel.For(0, 4, _ =>
            //{
            //    var threadId = Thread.CurrentThread.ManagedThreadId;

            //    var value = $"This is data from thread {threadId}";

            //    _normalStatic = value;
            //    //_threadStatic = value;
            //    //CallContext.SetData("value", value);
            //    //_threadLocal.Value = value;
            //    //_asyncLocal.Value = value;
            //    Thread.Sleep(200);
            //    Console.WriteLine($"Use Normal; Thread: {threadId}; Value: {_normalStatic}");
            //    //Console.WriteLine($"Use ThreadStaticAttribute; Thread: {threadId}; Value: {_threadStatic}");
            //    //Console.WriteLine($"Use CallContext; Thread: {threadId}; Value: {CallContext.GetData("value")}");
            //    //Console.WriteLine($"Use ThreadLocal; Thread:{threadId}; Value:{_threadLocal.Value}");
            //    //Console.WriteLine($"Use AsyncLocal; Thread: {threadId}; Value: {_asyncLocal.Value}");
            //});

            Console.Read();
        }
    }

}
