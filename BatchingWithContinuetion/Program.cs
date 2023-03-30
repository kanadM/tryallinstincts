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
            string startDate = "2023-03-24";
            string endDate = "2023-03-30";
            string daysPerBatch = "1";
            string lastRunId = "230330134257_230330_230331";
            //string lastRunId = "230330134257_230327_230328";
            var job = new ParsedDataDiffJob();
            job.Execute(daysPerBatch, startDate, endDate, lastRunId);
            Console.ReadLine();
        }
         
    }
}
