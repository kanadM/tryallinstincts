using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ConsoleApp1
{

    public interface ILogger
    {
        void LogInfo(string message);
    }

    public class Logger : ILogger
    { 
        public void LogInfo(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class TimeLoger : IDisposable
    {
        Stopwatch sw = new Stopwatch();

        ILogger _logger;
        string _flowName;
        public TimeLoger(ILogger logger, string flowName)
        {
            _logger = logger;
            _flowName = flowName;
            sw.Start();
        }

        public void Dispose()
        {
            _logger.LogInfo($"{_flowName} took {sw.Elapsed.TotalMilliseconds} ms.");
        }
    }
}
