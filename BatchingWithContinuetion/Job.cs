using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchingWithContinuetion
{
    public interface IJob
    {
        void Execute(params string[] args);
    }
    public class ParsedDataDiffJob : IJob
    {
        public IEnumerable<DateRange> GetBatches(DateTime startDate, DateTime endDate, int daysPerBatch)
        {

            if (startDate > endDate)
                throw new ArgumentException("start date can not be greater than end date");

            DateTime batchStartDate = startDate;
            DateTime batchEndDate;
            bool timeToExit = false;
            do
            {
                batchEndDate = batchStartDate.AddDays(daysPerBatch);

                if (batchEndDate >= endDate)
                {
                    batchEndDate = endDate;
                    timeToExit = true;
                }

                yield return new DateRange(batchStartDate, batchEndDate);

                batchStartDate = batchEndDate;

            } while (!timeToExit);
        }
        /// <summary>
        ///     args[0] : Batchsize Number of days per batch eg:- if args[0] = 1 then it will create and push for per day basis
        ///     Output file on s3 will have name like <executionStartDate>_<batchStartDate>_<batchEndDate> i.e.LastRunId
        ///     args[1] : StartDate  
        ///     args[2] : EndDate  
        ///     args[3] : LastRunId  <executionStartDate>_<batchStartDate>_<batchEndDate>
        /// </summary>
        /// <param name="args"></param>
        public void Execute(params string[] args)
        {
            const string DATEFORMAT = "yyMMdd";
            var today = DateTime.Now.ToString("yyMMddHHmmss");
            // Set batch size and delay time
            int daysPerBatch = Convert.ToInt32(args[0]); //100
            DateTime startDate = Convert.ToDateTime(args[1]);
            DateTime endDate = Convert.ToDateTime(args[2]).AddDays(1);

            string lastRunId = args.Length == 4 ? args[3] : null; //<runDate:yyyyyMMDDhhmmsstt>_<startDate>_<endDate>

            string currentRunIdFormat;
            if (string.IsNullOrWhiteSpace(lastRunId))
                currentRunIdFormat = $"{today}_{{0}}_{{1}}";
            else
            {
                currentRunIdFormat = $"{lastRunId.Split('_')[0]}_{{0}}_{{1}}";
                startDate = DateTime.ParseExact(lastRunId.Split('_')[2], DATEFORMAT, System.Globalization.CultureInfo.InvariantCulture);
            }

            if (startDate >= endDate)
            {
                throw new ArgumentException("Either provided date range is not correct, or excution was completed successfully. if you want execute for " +
                    "same date range again re-run this job without setting LastRunId parameter");
            } 

       
            foreach (var range in GetBatches(startDate, endDate, daysPerBatch))
            {
                string currentRunId = string.Format(currentRunIdFormat, range.StartDate.ToString(DATEFORMAT), range.EndDate.ToString(DATEFORMAT));
                Console.WriteLine($"Executing diff logic for batch with StartDate : {range.StartDate.ToString(DATEFORMAT)} \t EndDate {range.EndDate.ToString(DATEFORMAT)}");
                //code to featch data from REDSHIFT db
                //code to evaluate difference w.r.t SQL db
                //code to upload result set to S3
                System.Threading.Thread.Sleep(5000);
                Console.Write($"Execution done");
                
                //Once execution is done
                //code to log last successfull run id into Logs or send email for last successful run.
                File.Create($"D:\\Batching\\{currentRunId}.txt");
            }
        }
    }
    public class DateRange
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateRange(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
    }
    // Sample record class
    class Record
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
    }
}


