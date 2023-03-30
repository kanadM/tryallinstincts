using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net4App
{
    public class Profiler : IDisposable
    {
        private SolveLogger _logger => SolveLogger.GetCurrent();

        private readonly Stopwatch _watch;
        private readonly string _name;
        public Profiler(string name)
        {
            _name = name;
            _watch = new Stopwatch();
            _watch.Start();
        }

        public void Dispose()
        {
            var timeTakenInMs = _watch.ElapsedMilliseconds;
            _logger.Info($"completed_{_name}_{timeTakenInMs}");
        }
    }
}
