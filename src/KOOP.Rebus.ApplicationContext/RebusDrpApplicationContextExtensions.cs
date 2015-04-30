using System;
using Rebus.Configuration;
using Rebus.KOOP.DRP.ApplicationContext.Configuration;

namespace Rebus.KOOP.DRP.ApplicationContext
{
    public static class RebusDrpApplicationContextExtensions
    {
        public static RebusConfigurer EnableTransferDrpApplicationContext(this RebusConfigurer configurer, Action<ApplicationContextOptions> configure)
        {
            if (configurer == null) throw new ArgumentNullException("configurer");
            if (configurer.Backbone == null) throw new InvalidOperationException("configurer must have a backbone");

            var identityConfigurer = configurer.Backbone.LoadFromRegistry(() => new ApplicationContextConfigurer(configurer.Backbone));

            configure(new ApplicationContextOptions(identityConfigurer));

            return configurer;
        }
        
        public static RebusConfigurer EnableTransferDrpApplicationContext(this RebusConfigurer configurer)
        {
            if (configurer == null) throw new ArgumentNullException("configurer");
            if (configurer.Backbone == null) throw new InvalidOperationException("configurer must have a backbone");

            configurer.Backbone.LoadFromRegistry(() => new ApplicationContextConfigurer(configurer.Backbone));            

            return configurer;
        }
    }
}