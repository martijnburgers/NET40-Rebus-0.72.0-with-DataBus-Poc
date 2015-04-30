using System.Collections.Generic;
using Microsoft.IdentityModel.Claims;


namespace Rebus.IdentityClaims
{
    public interface IIdentityClaimsResolver
    {
        IEnumerable<Claim> GetClaims();
    }
}   