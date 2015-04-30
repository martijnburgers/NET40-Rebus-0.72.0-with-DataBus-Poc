using System;

namespace Rebus.KOOP.DRP.ApplicationContext.Configuration
{
    public class ApplicationContextResolverContext
    {
        private readonly ApplicationContextConfigurer _configurer;

        public ApplicationContextResolverContext(ApplicationContextConfigurer configurer)
        {
            if (configurer == null) throw new ArgumentNullException("configurer");
            _configurer = configurer;
        }

        public IApplicationContextResolver ResolveApplicationContextResolver()
        {
            return _configurer.ApplicationContextResolver;
        }
    }
}