using ConfigLoader;
using Dasync.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace net4App
{
    class Program
    {
        //public static string PATH_TO_CONFIG = @"C:\Users\Aumni\source\repos\ConsoleApp1\net4App\logger.app.config";

        // static void Main(string[] args)
        // {
        //     AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", PATH_TO_CONFIG);


        //     var title = ConfigurationManager.AppSettings["filename"];
        //     //var config = WebConfigurationManager.OpenWebConfiguration(PATH_TO_CONFIG);

        //     //title = config.AppSettings.Settings["filename"].Value;
        //     //CustomConfigurationLoader.GetConfigurationValue();
        //     Console.WriteLine($"Config file name : {title}");
        //     Console.ReadKey();
        // }

        static async Task Main(string[] args)
        {
            //BatchingLogicCheck.Execute();
            await AsyncAndThreadLocal.ExecuteAsync();
            Console.ReadLine();
        }

        #region TestConcurrentBag
        static void perfTest()
        {
            Console.WriteLine($"Max generation:{GC.MaxGeneration}");
            var tm = GC.GetTotalMemory(false);
            Console.WriteLine($"A:{tm}");

            List<PortfolioSecurity1> portfolioSecurities = new List<PortfolioSecurity1>();
            for (int i = 0; i < 10000; i++)
            {
                portfolioSecurities.Add(new PortfolioSecurity1
                {
                    PortfolioSecurityId = 8502 + i,
                    PortfolioId = 9999,
                    SecurityId = 8502 + i,
                    IdentifierDisplay = "asdf",
                    OrderIndex = null,
                    SecurityNameDisplay = "asdf asdfasddfwe",
                });
            }

            tm = GC.GetTotalMemory(false);
            Console.WriteLine($"B:{tm}");

            var task = TranslateAsync(portfolioSecurities);
            task.Wait();
            var result = task.Result?.Count;
            Console.WriteLine($"Done for task 1 count:{result}!");
            tm = GC.GetTotalMemory(false);
            Console.WriteLine($"C:{tm}");

            var task2 = Task.Factory.StartNew(() => Translate(portfolioSecurities));
            task.Wait();
            result = task.Result?.Count;
            Console.WriteLine($"Done for task 2 count:{result}!");
            tm = GC.GetTotalMemory(false);
            Console.WriteLine($"D:{tm}");
            result = task.Result?.Count;

            GC.Collect();
            tm = GC.GetTotalMemory(false);
            Console.WriteLine($"E:{tm}");
            result = null;
            tm = GC.GetTotalMemory(false);
            Console.WriteLine($"F:{tm}");
            GC.Collect();
            tm = GC.GetTotalMemory(false);
            Console.WriteLine($"G:{tm}");
            Console.WriteLine($"Max generation:{GC.MaxGeneration}");
            Console.ReadKey();
        }

        private static async Task<ConcurrentBag<PortfolioSecurity>> TranslateAsync(List<PortfolioSecurity1> portfolioSecurities)
        {
            var sw = Stopwatch.StartNew();
            ConcurrentBag<PortfolioSecurity> response = default;

            if (portfolioSecurities != default)
            {
                response = new ConcurrentBag<PortfolioSecurity>();

                await portfolioSecurities.ParallelForEachAsync(async (x) =>
                {
                    response.Add(new PortfolioSecurity
                    {
                        PortfolioId = x.PortfolioId,
                        PortfolioSecurityId = x.PortfolioSecurityId,
                        SecurityId = x.SecurityId,
                        IdentifierDisplay = x.IdentifierDisplay,
                        OrderIndex = x.OrderIndex,
                        SecurityNameDisplay = x.SecurityNameDisplay
                    });
                }, 4);
            }
            Console.WriteLine($"TimeTaken: {(sw.ElapsedMilliseconds)} for Total Processing of {response.Count()} sources.");

            return response;
        }

        private static ConcurrentBag<PortfolioSecurity> Translate(List<PortfolioSecurity1> portfolioSecurities)
        {
            var sw = Stopwatch.StartNew();
            ConcurrentBag<PortfolioSecurity> response = default;

            if (portfolioSecurities != default)
            {
                response = new ConcurrentBag<PortfolioSecurity>();

                foreach (var x in portfolioSecurities)
                {
                    response.Add(new PortfolioSecurity
                    {
                        PortfolioId = x.PortfolioId,
                        PortfolioSecurityId = x.PortfolioSecurityId,
                        SecurityId = x.SecurityId,
                        IdentifierDisplay = x.IdentifierDisplay,
                        OrderIndex = x.OrderIndex,
                        SecurityNameDisplay = x.SecurityNameDisplay
                    });
                }
            }
            Console.WriteLine($"TimeTaken: {(sw.ElapsedMilliseconds)} for Total Processing of {response.Count()} sources.");

            return response;
        }
        [Serializable()]
        public class BaseModel : INotifyPropertyChanged
        {
            [field: NonSerialized]
            public event PropertyChangedEventHandler PropertyChanged;
            public void NotifyPropertyChanged(String info)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
            }

            protected void SetProperty<T>(string propertyName, ref T propValue, T newValue)
            {
                if (
                    ((propValue == null) && (newValue != null))
                    || ((propValue != null) && (newValue == null))
                    || ((propValue != null) && !EqualityComparer<T>.Default.Equals(propValue, newValue))
                    )
                {
                    propValue = newValue;
                    NotifyPropertyChanged(propertyName);
                }
            }

        }

        [Serializable]

        public class PortfolioSecurity1 : BaseModel
        {
            private int _PortfolioSecurityId;
            public int PortfolioSecurityId
            {
                get { return _PortfolioSecurityId; }
                set { SetProperty("PortfolioSecurityId", ref _PortfolioSecurityId, value); }
            }
            private int _PortfolioId;
            public int PortfolioId
            {
                get { return _PortfolioId; }
                set { SetProperty("PortfolioId", ref _PortfolioId, value); }
            }
            private int _SecurityId;
            public int SecurityId
            {
                get { return _SecurityId; }
                set { SetProperty("SecurityId", ref _SecurityId, value); }
            }
            private string _IdentifierDisplay;
            public string IdentifierDisplay
            {
                get { return _IdentifierDisplay; }
                set { SetProperty("IdentifierDisplay", ref _IdentifierDisplay, value); }
            }
            private int? _OrderIndex;
            public int? OrderIndex
            {
                get { return _OrderIndex; }
                set { SetProperty("OrderIndex", ref _OrderIndex, value); }
            }

            private string _SecurityNameDisplay;
            public string SecurityNameDisplay
            {
                get { return _SecurityNameDisplay; }
                set { SetProperty(nameof(SecurityNameDisplay), ref _SecurityNameDisplay, value); }
            }

            public PortfolioSecurity1()
            {

            }

            public PortfolioSecurity1(int portfolioSecurityId, int portfolioId, int securityId, string identifierDisplay, int? orderIndex, string securityNameDisplay)
            {
                this.PortfolioSecurityId = portfolioSecurityId;
                this.PortfolioId = portfolioId;
                this.SecurityId = securityId;
                this.IdentifierDisplay = identifierDisplay;
                this.OrderIndex = orderIndex;
                this.SecurityNameDisplay = securityNameDisplay;
            }
        }
        public class PortfolioSecurity
        {
            public int PortfolioSecurityId { get; set; }
            public int PortfolioId { get; set; }
            public int SecurityId { get; set; }
            public string IdentifierDisplay { get; set; }
            public int? OrderIndex { get; set; }
            public string SecurityNameDisplay { get; set; }
        }
        #endregion
    }
}
