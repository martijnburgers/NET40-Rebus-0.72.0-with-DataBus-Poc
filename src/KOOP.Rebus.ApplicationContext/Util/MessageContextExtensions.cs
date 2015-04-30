using Rebus.KOOP.DRP.ApplicationContext.Configuration;

namespace Rebus.KOOP.DRP.ApplicationContext.Util
{
    public static class MessageContextExtensions
    {
        public static string GetApplicationContext(this IMessageContext messageContext)
        {
            if (messageContext.Headers.ContainsKey(ApplicationContextConfigurer.ApplicationContextHeaderKey))
            {
                return messageContext.Headers[ApplicationContextConfigurer.ApplicationContextHeaderKey] as string;
            }

            return null;
        }
    }
}
