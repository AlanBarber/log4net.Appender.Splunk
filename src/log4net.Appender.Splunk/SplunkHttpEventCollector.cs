using log4net.Core;
using Splunk.Logging;
using System;
using System.Dynamic;
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
                new Uri(ServerUrl),                                                                     // Splunk HEC URL
                Token,                                                                                  // Splunk HEC token *GUID*
                new HttpEventCollectorEventInfo.Metadata(null, null, "_json", Environment.MachineName), // Metadata
                HttpEventCollectorSender.SendMode.Sequential,                                           // Sequential sending to keep message in order
                0,                                                                                      // BatchInterval - Set to 0 to disable
                0,                                                                                      // BatchSizeBytes - Set to 0 to disable
                0,                                                                                      // BatchSizeCount - Set to 0 to disable
                new HttpEventCollectorResendMiddleware(RetriesOnError).Plugin                           // Resend Middleware with retry
            );

            // throw error on send failure
            _hecSender.OnError += exception =>
            {
                throw new Exception($"SplunkHttpEventCollector failed to send log event to Splunk server '{new Uri(ServerUrl).Authority}' using token '{Token}'. Exception: {exception}");
            };

            // If enabled will create callback to bypass ssl error checks for our server url
            if (IgnoreSslErrors)
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
                {
                    var httpWebRequest = sender as HttpWebRequest;
                    return httpWebRequest?.RequestUri.Authority == new Uri(ServerUrl).Authority;
                };
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
            var metaData = new HttpEventCollectorEventInfo.Metadata(null, loggingEvent.LoggerName, "_json", Environment.MachineName);

            // Build optional data object
            dynamic objData = null;

            if (loggingEvent.ExceptionObject != null)
            {
                objData = new ExpandoObject();

                if (loggingEvent.ExceptionObject != null)
                {
                    objData.Exception = loggingEvent.ExceptionObject;
                }
            }

            // Send the event to splunk
            _hecSender.Send(Guid.NewGuid().ToString(), loggingEvent.Level.Name, loggingEvent.RenderedMessage, objData, metaData);
            _hecSender.FlushSync();
        }
    }
}
