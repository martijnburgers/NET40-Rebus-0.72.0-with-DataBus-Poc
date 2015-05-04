using System.Collections.Generic;
using Microsoft.IdentityModel.Claims;
using Newtonsoft.Json;
using Rebus.IdentityClaims.Configuration;
using Rebus.IdentityClaims.Serialization;

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
            var jsonResolver = new IgnorableSerializerContractResolver();
            
            // ignore single property
            jsonResolver.Ignore(typeof(Claim), "Subject");
            
            var jsonSettings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, ContractResolver = jsonResolver };

            string serializedClaims = JsonConvert.SerializeObject(
                claims,
                jsonSettings);

            bus.AttachHeader(message, headerKey ?? IdentityConfigurer.IdentityClaimsHeaderKey, serializedClaims);
        }
    }
}