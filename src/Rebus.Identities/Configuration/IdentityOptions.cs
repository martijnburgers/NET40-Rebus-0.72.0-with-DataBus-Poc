using System;

namespace Rebus.IdentityClaims.Configuration
{
    public class IdentityOptions
    {
        private readonly IdentityConfigurer _identityConfigurer;

        internal IdentityOptions(IdentityConfigurer identityConfigurer)
        {
            _identityConfigurer = identityConfigurer;
        }

        public IdentityOptions ReceiveOnly()
        {
            _identityConfigurer.ReceiveOnly();

            return this;
        }

        public void UseIdentityClaimsResolver(
            Func<RebusIdentityResolverContext, IIdentityClaimsResolver> factory)
        {
            _identityConfigurer.UseIdentityClaimsResolver(factory);            
        }             

        public void UseServiceLocator()
        {
            _identityConfigurer.UseServiceLocator();
        }        
    }
}