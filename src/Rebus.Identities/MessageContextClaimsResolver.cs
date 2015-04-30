using System.Collections.Generic;
using Microsoft.IdentityModel.Claims;
using Rebus.IdentityClaims.Configuration;
using Rebus.Logging;

namespace Rebus.IdentityClaims
{
    public class MessageContextClaimsResolver : IIdentityClaimsResolver
    {
        private static ILog _log;

        static MessageContextClaimsResolver()
        {
            RebusLoggerFactory.Changed += f => _log = f.GetCurrentClassLogger();
        }

        public IEnumerable<Claim> GetClaims()
        {
            IMessageContext currentMessageContext = MessageContext.GetCurrent();


            if (currentMessageContext == null)
            {
                _log.Warn("No message context found.");

                return null;
            }

            if (!currentMessageContext.Items.ContainsKey(IdentityConfigurer.IdentityClaimsItemKey))
            {
                _log.Warn("No message context claim items found.");

                return null;
            }

            return currentMessageContext.Items[IdentityConfigurer.IdentityClaimsItemKey] as IEnumerable<Claim>;
        }
    }
}