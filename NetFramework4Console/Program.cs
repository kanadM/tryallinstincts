using System;
using System.Linq;
using System.Text;
using System.Threading;

namespace NetFramework4Console
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 1000; i++)
            {
                var loopvariable = i; 
                AsyncTasks.Run(() =>
                {
                    Thread.Sleep(1);
                    Console.WriteLine("loopvariable: \t" + loopvariable);
                }, "logging");
            }
            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}
