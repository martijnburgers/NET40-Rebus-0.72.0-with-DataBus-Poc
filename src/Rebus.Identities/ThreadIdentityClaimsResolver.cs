using System.Collections.Generic;
using System.Threading;
using Microsoft.IdentityModel.Claims;
using Rebus.Logging;

namespace Rebus.IdentityClaims
{
    public class ThreadIdentityClaimsResolver : IIdentityClaimsResolver
    {
        private static ILog _log;

        static ThreadIdentityClaimsResolver()
        {
            RebusLoggerFactory.Changed += f => _log = f.GetCurrentClassLogger();
        }

        public IEnumerable<Claim> GetClaims()
        {
            ClaimsPrincipal claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;

            if (claimsPrincipal == null)
            {
                _log.Warn("The current principal on the thread is not a claimsprincipal.");

                return null;
            }

            if (claimsPrincipal.Identity == null)
            {
                _log.Warn("The current principal on the thread has no identity.");

                return null;
            }
            
            var claimsIdentity = claimsPrincipal.Identity as ClaimsIdentity;

            if (claimsIdentity == null)
            {
                _log.Warn("The current principal on the thread is no claims identity.");

                return null;
            }

            return claimsIdentity.Claims;
        }
    }
}