using System;
using System.Runtime.Serialization;

namespace Rebus.KOOP.DRP.ApplicationContext.Configuration
{
    public class RebusApplicationContextConfigurationException : RebusApplicationContextException
    {
        public RebusApplicationContextConfigurationException()
        {
        }

        public RebusApplicationContextConfigurationException(string message)
            : base(message)
        {
        }

        public RebusApplicationContextConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected RebusApplicationContextConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}