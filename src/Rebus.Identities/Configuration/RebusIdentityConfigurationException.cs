using System;
using System.Runtime.Serialization;

namespace Rebus.IdentityClaims.Configuration
{
    public class RebusIdentityConfigurationException : RebusIdentityException
    {
        public RebusIdentityConfigurationException()
        {
        }

        public RebusIdentityConfigurationException(string message)
            : base(message)
        {
        }

        public RebusIdentityConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected RebusIdentityConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}