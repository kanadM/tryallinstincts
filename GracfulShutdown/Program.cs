using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Serilog.Formatting.Json;

namespace GracfulShutdown
{
    class Program
    {
        static Program()
        {
            var loggerConfiguration = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .Enrich.FromLogContext()
           .WriteTo.File("logs/myapp.log",
               fileSizeLimitBytes: 1000000,
               rollOnFileSizeLimit: true,
               rollingInterval: RollingInterval.Day,
               retainedFileCountLimit: 7,
               encoding: System.Text.Encoding.UTF8,
               outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
           .WriteTo.File(new JsonFormatter(), "logs/myapp.json", rollingInterval: RollingInterval.Day);

            Log.Logger = loggerConfiguration.CreateLogger();
        }


        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        // Define the delegate that will handle the system shutdown event
        private delegate bool EventHandler(CtrlType sig);


        // This method will be called when a system shutdown event is detected
        private static bool OnSystemShutdown(CtrlType sig)
        {
            Log.Information($"{DateTime.Now:hh:mm:ss:fff} System shutdown detected. {sig.ToString()}");
            // TODO: Add your shutdown code here
            resetEvent.Wait(15000); // Wait for up to 15 seconds
            return true;
        }

        // Define the possible system shutdown events
        private enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }
        static ManualResetEventSlim resetEvent = new ManualResetEventSlim(false);

        public static IntPtr OnSystemShutdown1 { get; private set; }

        static async Task Main(string[] args)
        {

            SetConsoleCtrlHandler(new EventHandler(OnSystemShutdown), true);

            //AppDomain.CurrentDomain.ProcessExit += new System.EventHandler(OnApplicationExist); 

            var cancellationTokenSource = new CancellationTokenSource();

            // Handle Ctrl+C
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                Log.Information($"{DateTime.Now:hh:mm:ss:fff}:Console.CancelKeyPress.");
                cancellationTokenSource.Cancel();
                resetEvent.Wait(15000); // Wait for up to 15 seconds

            };
            Process.GetCurrentProcess().MainWindowHandle += OnSystemShutdown1
            // Handle unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                Log.Error(eventArgs.ExceptionObject as Exception, $"{DateTime.Now:hh:mm:ss:fff}:Unhandled exception");
                cancellationTokenSource.Cancel();
            };

            // Handle task cancellation
            cancellationTokenSource.Token.Register(() =>
            {
                Log.Information($"{DateTime.Now:hh:mm:ss:fff}:cancellationTokenSource.Token.Register: Cancellation requested.");
            });

            // Handle process exit
            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
            {
                Log.Information($"{DateTime.Now:hh:mm:ss:fff}:Process is exiting");
                resetEvent.Wait(15000); // Wait for up to 15 seconds
                cancellationTokenSource.Cancel();
            };

            try
            {
                await DoWorkAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Log.Information($"{DateTime.Now:hh:mm:ss:fff}:Operation canceled.");
            }
            catch (Exception ex)
            {
                Log.Error($"{DateTime.Now:hh:mm:ss:fff}:Error: {ex.Message}");
            }
            finally
            {
                Console.WriteLine($"{DateTime.Now:hh:mm:ss:fff}:Exiting application.");
                Log.CloseAndFlush();
                //resetEvent.Set();
            }
        }

        private static void OnApplicationExist(object sender, EventArgs e)
        {
            Log.Information($"{DateTime.Now:hh:mm:ss:fff}:OnApplicationExist.");
            resetEvent.Wait(15000); // Wait for up to 15 seconds
        }

        static async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine($"{DateTime.Now:hh:mm:ss:fff}: Working...");
                await Task.Delay(10000, cancellationToken);
            }
        }
    }
}
