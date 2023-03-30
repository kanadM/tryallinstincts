using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net4App
{
    public class SolveLogger
    {
        private static SolveLogger _solveLogger;
        private SolveLogger()
        {

        }
        private static Object getLock = new object();
        public static SolveLogger GetCurrent()
        {
            if (_solveLogger == null)
            {
                lock (getLock)
                {
                    if (_solveLogger == null)
                    {
                        _solveLogger = new SolveLogger();
                    }
                }
            }
            return _solveLogger;
        }
        public void Info(string message)
        {
            Console.WriteLine($"{DateTime.Now}:{message}");
        }
    }
}
