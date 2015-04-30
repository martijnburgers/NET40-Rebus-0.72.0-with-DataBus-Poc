using System;
using System.Runtime.Serialization;

namespace Rebus.IdentityClaims
{
    public class RebusIdentityException : Exception
    {
        public RebusIdentityException()
        {
        }

        public RebusIdentityException(string message) : base(message)
        {
        }

        public RebusIdentityException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RebusIdentityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}