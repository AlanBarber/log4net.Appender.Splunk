using log4net.Core;
using Splunk.Logging;
using System;
using System.Collections.Generic;

namespace log4net.Appender.Splunk
{
    /// <summary>
    /// log4net Appender
    /// </summary>
    public class SplunkHttpEventCollector : AppenderSkeleton
    {
        private HttpEventCollectorSender _hecSender;
        public string ServerUrl { get; set; }
        public string Token { get; set; }
        public int RetriesOnError { get; set; } = 0;

        /// <summary>
        /// This appender requires a <see cref="Layout"/> to be set.
        /// </summary>
        protected override bool RequiresLayout => true;

        /// <summary>
        /// Initialize the options on the SplunkHttpEventCollector appender
        /// </summary>
        public override void ActivateOptions()
        {
            _hecSender = new HttpEventCollectorSender(
                new Uri(ServerUrl),                                                                 // Splunk HEC URL
                Token,                                                                              // Splunk HEC token *GUID*
                new HttpEventCollectorEventInfo.Metadata(null, null, "_json", GetMachineName()),    // Metadata
                HttpEventCollectorSender.SendMode.Sequential,                                       // Sequential sending to keep message in order
                0,                                                                                  // BatchInterval - Set to 0 to disable
                0,                                                                                  // BatchSizeBytes - Set to 0 to disable
                0,                                                                                  // BatchSizeCount - Set to 0 to disable
                new HttpEventCollectorResendMiddleware(RetriesOnError).Plugin                       // Resend Middleware with retry
            );
        }

        /// <summary>
        /// Method used by log4net to perform actual logging
        /// </summary>
        /// <param name="loggingEvent"></param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            // Sanity check for LogEventInfo
            if (loggingEvent == null)
            {
                throw new ArgumentNullException(nameof(loggingEvent));
            }

            // Make sure we have a properly setup HttpEventCollectorSender
            if (_hecSender == null)
            {
                throw new Exception("SplunkHttpEventCollector Append() called before ActivateOptions()");
            }

            // Build metaData
            var metaData = new HttpEventCollectorEventInfo.Metadata(null, loggingEvent.LoggerName, "_json", GetMachineName());

            // Build properties object and assign standard values
            var properties = new Dictionary<String, object>
            {
                {"Source", loggingEvent.LoggerName},
                { "Host", GetMachineName()}
            };

            // Get properties from event
            if (loggingEvent.Properties != null && loggingEvent.Properties.Count > 0)
            {
                foreach (var key in loggingEvent.Properties.GetKeys())
                {
                    properties.Add(key, loggingEvent.Properties[key]);
                }
            }

            // Send the event to splunk
            _hecSender.Send(null, loggingEvent.Level.Name, null, loggingEvent.RenderedMessage, loggingEvent.ExceptionObject, properties, metaData);
            _hecSender.FlushSync();
        }

        /// <summary>
        /// Gets the machine name
        /// </summary>
        /// <returns></returns>
        private string GetMachineName()
        {
            return !string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("COMPUTERNAME")) ? System.Environment.GetEnvironmentVariable("COMPUTERNAME") : System.Net.Dns.GetHostName();
        }
    }
}
