using System;
using Rebus.Configuration;
using Rebus.IdentityClaims.Configuration;

namespace Rebus.IdentityClaims
{
    public static class RebusIdentityExtensions
    {
        public static RebusConfigurer EnableIdentityClaims(this RebusConfigurer configurer, Action<IdentityOptions> configure)
        {
            if (configurer == null) throw new ArgumentNullException("configurer");
            if (configurer.Backbone == null) throw new InvalidOperationException("configurer must have a backbone");

            var identityConfigurer = configurer.Backbone.LoadFromRegistry(() => new IdentityConfigurer(configurer.Backbone));

            configure(new IdentityOptions(identityConfigurer));

            return configurer;
        }
        
        public static RebusConfigurer EnableIdentityClaims(this RebusConfigurer configurer)
        {
            if (configurer == null) throw new ArgumentNullException("configurer");
            if (configurer.Backbone == null) throw new InvalidOperationException("configurer must have a backbone");

            configurer.Backbone.LoadFromRegistry(() => new IdentityConfigurer(configurer.Backbone));            

            return configurer;
        }
    }
}