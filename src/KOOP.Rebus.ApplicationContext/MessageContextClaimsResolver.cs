using System;
using Rebus.KOOP.DRP.ApplicationContext.Configuration;
using Rebus.Logging;

namespace Rebus.KOOP.DRP.ApplicationContext
{
    public class MessageHeaderApplicationContextResolver : IApplicationContextResolver
    {
        private static ILog _log;

        static MessageHeaderApplicationContextResolver()
        {
            RebusLoggerFactory.Changed += f => _log = f.GetCurrentClassLogger();
        }

        public string GetApplicationContext()
        {
            IMessageContext currentMessageContext = MessageContext.GetCurrent();


            if (currentMessageContext == null)
            {
                _log.Warn("No message context found.");

                return null;
            }

            if (!currentMessageContext.Headers.ContainsKey(ApplicationContextConfigurer.ApplicationContextHeaderKey))
            {
                _log.Warn(String.Format("No message header '{0}' found in message context.", ApplicationContextConfigurer.ApplicationContextHeaderKey));

                return null;
            }

            return currentMessageContext.Headers[ApplicationContextConfigurer.ApplicationContextHeaderKey] as string;
        }
    }
}