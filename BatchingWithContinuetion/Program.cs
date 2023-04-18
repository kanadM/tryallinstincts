using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace BatchingWithContinuetion
{
    class Program
    {
        static void Main(string[] args)
        {
            string startDate = "2023-03-30";
            string endDate = "2023-03-30";
            string daysPerBatch = "2";
            string lastRunId = null;// "230330142002_230324_230326";
            var job = new ParsedDataDiffJob();
            job.Execute(daysPerBatch, startDate, endDate, lastRunId);
            Console.ReadLine();
        }
         
    }
}
