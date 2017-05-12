using log4net.Core;
using Splunk.Logging;
using System;
using System.Collections.Generic;
using System.Net;

namespace log4net.Appender.Splunk
{
    public class SplunkHttpEventCollector : AppenderSkeleton
    {
        private HttpEventCollectorSender _hecSender;
        public string ServerUrl { get; set; }
        public string Token { get; set; }
        public int RetriesOnError { get; set; } = 0;
        public bool IgnoreSslErrors { get; set; } = false;

        /// <summary>
        /// This appender requires a <see cref="Layout"/> to be set.
        /// </summary>
        protected override bool RequiresLayout => true;

        /// <summary>
        /// 
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

            // throw error on send failure
            _hecSender.OnError += exception =>
            {
                throw new Exception($"SplunkHttpEventCollector failed to send log event to Splunk server '{new Uri(ServerUrl).Authority}' using token '{Token}'. Exception: {exception}");
            };

            // If enabled will create callback to bypass ssl error checks for our server url
            if (IgnoreSslErrors)
            {
                // TODO: Enable sql error bypass
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggingEvent"></param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            SendEventToServer(loggingEvent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loggingEvent"></param>
        private void SendEventToServer(LoggingEvent loggingEvent)
        {
            // Sanity check for LogEventInfo
            if (loggingEvent == null)
            {
                throw new ArgumentNullException(nameof(loggingEvent));
            }

            // Make sure we have a properly setup HttpEventCollectorSender
            if (_hecSender == null)
            {
                throw new Exception("SplunkHttpEventCollector SendEventToServer() called before InitializeTarget()");
            }

            // Build metaData
            var metaData = new HttpEventCollectorEventInfo.Metadata(null, loggingEvent.LoggerName, "_json", GetMachineName());

            // Build properties object
            var properties = new Dictionary<String, object>();

            // Add standard values to properties
            properties.Add("Source", loggingEvent.LoggerName);
            properties.Add("Host", GetMachineName());

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
