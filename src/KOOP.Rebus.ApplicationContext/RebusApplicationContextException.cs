using System;
using System.Runtime.Serialization;

namespace Rebus.KOOP.DRP.ApplicationContext
{
    public class RebusApplicationContextException : Exception
    {
        public RebusApplicationContextException()
        {
        }

        public RebusApplicationContextException(string message) : base(message)
        {
        }

        public RebusApplicationContextException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RebusApplicationContextException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}