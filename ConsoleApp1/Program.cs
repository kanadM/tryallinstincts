namespace ConsoleApp1
{
    public class Program
    {

        public static void GetConfigurationValue()
        {
            var title = ConfigurationManager.AppSettings["filename"];
            Console.WriteLine($"Config file name : {title}");
        }


        private static void loggerconfigChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("Something has changed");
        }
        public static void Main(string[] args)
        {
            GetConfigurationValue();
            Console.ReadKey();
            Console.WriteLine("watcher started");
            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Path = @"C:\Users\Aumni\source\repos\ConsoleApp1\MultiTasking";
            fileSystemWatcher.Filter = "logger.appsettings.json";
            fileSystemWatcher.Changed += loggerconfigChanged;
            fileSystemWatcher.EnableRaisingEvents = true;
            Console.ReadKey();
        }

    }
}
