using Rebus.KOOP.DRP.ApplicationContext.Configuration;

namespace Rebus.KOOP.DRP.ApplicationContext.Util
{
    public static class BusExtensions
    {
        public static void AttachApplicationContext(
            this IBus bus,
            object message,
            string applicationName,
            string headerKey = null)
        {
           

            bus.AttachHeader(message, headerKey ?? ApplicationContextConfigurer.ApplicationContextHeaderKey, applicationName);
        }
    }
}