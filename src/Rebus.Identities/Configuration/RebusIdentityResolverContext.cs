using System;

namespace Rebus.IdentityClaims.Configuration
{
    public class RebusIdentityResolverContext
    {
        private readonly IdentityConfigurer _configurer;

        public RebusIdentityResolverContext(IdentityConfigurer configurer)
        {
            if (configurer == null) throw new ArgumentNullException("configurer");
            _configurer = configurer;
        }

        public IIdentityClaimsResolver ResolveIdentityClaimResolver()
        {
            return _configurer.IdentityClaimsResolver;
        }
    }
}