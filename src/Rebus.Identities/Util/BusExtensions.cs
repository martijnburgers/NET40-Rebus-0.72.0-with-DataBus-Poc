using System.Collections.Generic;
using Microsoft.IdentityModel.Claims;
using Newtonsoft.Json;
using Rebus.IdentityClaims.Configuration;

namespace Rebus.IdentityClaims.Util
{
    public static class BusExtensions
    {
        public static void AttachClaims(
            this IBus bus,
            object message,
            IEnumerable<Claim> claims,
            string headerKey = null)
        {
            string serializedClaims = JsonConvert.SerializeObject(
                claims,
                new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});

            bus.AttachHeader(message, headerKey ?? IdentityConfigurer.IdentityClaimsHeaderKey, serializedClaims);
        }
    }
}