using System;

namespace Rebus.KOOP.DRP.ApplicationContext.Configuration
{
    public class ApplicationContextOptions
    {
        private readonly ApplicationContextConfigurer _applicationContextConfigurer;

        internal ApplicationContextOptions(ApplicationContextConfigurer applicationContextConfigurer)
        {
            _applicationContextConfigurer = applicationContextConfigurer;
        }
        
        public void UseApplicationContextResolver(
            Func<ApplicationContextResolverContext, IApplicationContextResolver> factory)
        {
            _applicationContextConfigurer.UseApplicationContextResolver(factory);            
        }             

        public void UseServiceLocator()
        {
            _applicationContextConfigurer.UseServiceLocator();
        }        
    }
}