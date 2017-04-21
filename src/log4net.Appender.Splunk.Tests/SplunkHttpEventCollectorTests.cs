using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Dynamic;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace log4net.Appender.Splunk.Tests
{
    [TestClass]
    public class SplunkHttpEventCollectorTests
    {
        private log4net.ILog _logger = null;

        [TestInitialize]
        public void Initialize()
        {
            // Step 1. Create repository object
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

            // Step 2. Create appender
            var splunkHttpEventCollector = new SplunkHttpEventCollector();

            // Step 3. Set appender properties
            splunkHttpEventCollector.ServerUrl = "https://localhost:8088";
            splunkHttpEventCollector.Token = "1A29471E-3F18-4412-B032-80DD5712B691";
            splunkHttpEventCollector.RetriesOnError = 0;
            splunkHttpEventCollector.IgnoreSslErrors = true;

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

        [TestMethod]
        public void SendFatalWithException()
        {
            _logger.Fatal("This is a Fatal log message with an Exception", new Exception());

        }

        [TestMethod]
        public void SendFatalWithoutException()
        {
            _logger.Fatal("This is a Fatal log message without an Exception");
        }

        [TestMethod]
        public void SendErrorWithException()
        {
            _logger.Error("This is a Error log message with an Exception", new Exception());
        }

        [TestMethod]
        public void SendErrorWithoutException()
        {
            _logger.Error("This is a Error log message without an Exception");
        }

        [TestMethod]
        public void SendWarnWithException()
        {
            _logger.Warn("This is a Warn log message with an Exception", new Exception());
        }

        [TestMethod]
        public void SendWarnWithoutException()
        {
            _logger.Warn("This is a Warn log message without an Exception");
        }

        [TestMethod]
        public void SendInfoWithException()
        {
            _logger.Info("This is a Info log message with an Exception", new Exception());
        }

        [TestMethod]
        public void SendInfoWithoutException()
        {
            _logger.Info("This is a Info log message without an Exception");
        }

        [TestMethod]
        public void SendDebugWithException()
        {
            _logger.Debug("This is a Debug log message with an Exception", new Exception());
        }

        [TestMethod]
        public void SendDebugWithoutException()
        {
            _logger.Debug("This is a Debug log message without an Exception");
        }
    }
}
