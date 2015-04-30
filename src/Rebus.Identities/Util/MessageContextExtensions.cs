using System.Collections.Generic;
using Microsoft.IdentityModel.Claims;
using Rebus.IdentityClaims.Configuration;

namespace Rebus.IdentityClaims.Util
{
    public static class MessageContextExtensions
    {
        public static IEnumerable<Claim> GetIdentityClaims(this IMessageContext messageContext)
        {
            return messageContext.Items[IdentityConfigurer.IdentityClaimsItemKey] as IEnumerable<Claim>;
        }
    }
}
