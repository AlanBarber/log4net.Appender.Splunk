using System;
using System.IO;
using System.Linq;

namespace ConsoleTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SplunkHttpEventCollector log4net Appender Test App");

            // Get a logger instance
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));
            var logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            // Validate logger instance is loaded and display configuration
            Console.WriteLine("Logger Settings:");
            Console.WriteLine($"  Name:           {logger.Logger.Name}");
            Console.WriteLine($"  Root threshold: {logger.Logger.Repository.Threshold}");
            Console.WriteLine($"  IsFatalEnabled: {logger.IsFatalEnabled}");
            Console.WriteLine($"  IsErrorEnabled: {logger.IsErrorEnabled}");
            Console.WriteLine($"  IsWarnEnabled:  {logger.IsWarnEnabled}");
            Console.WriteLine($"  IsInfoEnabled:  {logger.IsInfoEnabled}");
            Console.WriteLine($"  IsDebugEnabled: {logger.IsDebugEnabled}");

            Console.WriteLine("  Appenders:");
            foreach (var a in logger.Logger.Repository.GetAppenders())
            {
                Console.WriteLine($"    {a.Name}");
            }

            Console.WriteLine("Writting log messages...");
            // Write a few messages
            logger.Debug("This is a debug log message");
            logger.Info("This is an info log message");
            logger.Warn("This is a warn log message");
            logger.Error("This is an error log message");
            logger.Fatal("This is a fatal log message");

            // Process an exception
            try
            {
                throw new Exception();
            }
            catch (Exception ex)
            {
                logger.Error("Our pretend exception detected!", ex);
            }

#if DEBUG
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
#endif
        }
    }
}
