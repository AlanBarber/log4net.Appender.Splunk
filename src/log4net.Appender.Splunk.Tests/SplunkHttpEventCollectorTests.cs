using log4net.Core;
using log4net.Repository.Hierarchy;
using System;
using Xunit;

namespace log4net.Appender.Splunk.Tests
{
    public class SplunkHttpEventCollectorTests
    {
        private log4net.ILog _logger = null;
        public SplunkHttpEventCollectorTests()
        {
            // Step 1. Create repository object
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

            // Step 2. Create appender
            var splunkHttpEventCollector = new SplunkHttpEventCollector();

            // Step 3. Set appender properties
            splunkHttpEventCollector.ServerUrl = "https://localhost:8088";
            splunkHttpEventCollector.Token = "ED9F5A37-BE9A-4782-B5F7-B6E31AC369CA";
            splunkHttpEventCollector.RetriesOnError = 0;

            log4net.Layout.PatternLayout patternLayout = new log4net.Layout.PatternLayout
            {
                ConversionPattern = "%message"
            };
            patternLayout.ActivateOptions();

            splunkHttpEventCollector.Layout = patternLayout;
            
            splunkHttpEventCollector.ActivateOptions();

            // Step 4. Add appender to logger
            hierarchy.Root.AddAppender(splunkHttpEventCollector);
            hierarchy.Threshold = Level.All;
            hierarchy.Configured = true;

            // Step 5. Create logger
            _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        [Fact]
        public void SendFatalWithException()
        {
            _logger.Fatal("This is a Fatal log message with an Exception", new Exception());
        }

        [Fact]
        public void SendFatalWithoutException()
        {
            _logger.Fatal("This is a Fatal log message without an Exception");
        }

        [Fact]
        public void SendErrorWithException()
        {
            _logger.Error("This is a Error log message with an Exception", new Exception());
        }

        [Fact]
        public void SendErrorWithoutException()
        {
            _logger.Error("This is a Error log message without an Exception");
        }

        [Fact]
        public void SendWarnWithException()
        {
            _logger.Warn("This is a Warn log message with an Exception", new Exception());
        }

        [Fact]
        public void SendWarnWithoutException()
        {
            _logger.Warn("This is a Warn log message without an Exception");
        }

        [Fact]
        public void SendInfoWithException()
        {
            _logger.Info("This is a Info log message with an Exception", new Exception());
        }

        [Fact]
        public void SendInfoWithoutException()
        {
            _logger.Info("This is a Info log message without an Exception");
        }

        [Fact]
        public void SendDebugWithException()
        {
            _logger.Debug("This is a Debug log message with an Exception", new Exception());
        }

        [Fact]
        public void SendDebugWithoutException()
        {
            _logger.Debug("This is a Debug log message without an Exception");
        }
    }
}
