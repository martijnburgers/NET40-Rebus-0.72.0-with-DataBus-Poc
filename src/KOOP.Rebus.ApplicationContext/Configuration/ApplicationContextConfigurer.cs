using System;
using Microsoft.Practices.ServiceLocation;
using Rebus.Configuration;
using Rebus.KOOP.DRP.ApplicationContext.Util;
using Rebus.Logging;

namespace Rebus.KOOP.DRP.ApplicationContext.Configuration
{
    public class ApplicationContextConfigurer : BaseConfigurer
    {        
        public const string ApplicationContextHeaderKey = "koop-drp-applicationcontext";
        private static ILog _log;
        
        private Func<ApplicationContextResolverContext, IApplicationContextResolver>
            _applicationContextResolverResolver = c => null;

        private IServiceLocator _serviceLocator;
        
        static ApplicationContextConfigurer()
        {
            RebusLoggerFactory.Changed += f => _log = f.GetCurrentClassLogger();
        }

        /// <summary>
        ///     CTOR
        /// </summary>
        /// <param name="backbone">The backbone of Rebus</param>
        public ApplicationContextConfigurer(ConfigurationBackbone backbone) : base(backbone)
        {
            TryConfigureServiceLocator();
            ConfigureEvents();
        }

        protected internal IApplicationContextResolver ApplicationContextResolver
        {
            get
            {
                return
                    GuardForResolvingNulls(                        
                            _applicationContextResolverResolver(new ApplicationContextResolverContext(this)));
            }
        }

        private static T GuardForResolvingNulls<T>(T objectToCheck) where T : class
        {
            if (objectToCheck == null)
                throw new RebusApplicationContextConfigurationException(
                    string.Format(
                        "Could not resolve a type implementing '{0}'. You are probably missing some configuration",
                        typeof (T).FullName));

            return objectToCheck;
        }

        private void TryConfigureServiceLocator()
        {
            //this is the only way to get to the container adapter, needed resolving services.
            _serviceLocator = Backbone.ActivateHandlers as IServiceLocator;
        }

        private void ConfigureEvents()
        {
            Backbone.ConfigureEvents(
                e =>
                {
                    e.MessageSent += (messageBus, destination, message) =>
                    {                        
                        AddApplicationContextToHeader(ApplicationContextResolver, messageBus, message);
                    };
                });
        }

        private static void AddApplicationContextToHeader(
            IApplicationContextResolver applicationContextResolver,
            IBus bus,
            object message)
        {
            string applicationContext = applicationContextResolver.GetApplicationContext();

            if (applicationContext == null)
            {
                _log.Warn(
                    "Resolved applicationcontext from resolver type '{0}' is null.",
                    applicationContextResolver.GetType());

                return;
            }

            bus.AttachApplicationContext(message, applicationContext);
        }

        protected internal void UseApplicationContextResolver(
            Func<ApplicationContextResolverContext, IApplicationContextResolver> factory)
        {
            if (factory == null) throw new ArgumentNullException("factory");

            _applicationContextResolverResolver = factory;
        }

        protected internal ApplicationContextConfigurer UseServiceLocator()
        {
            if (_serviceLocator == null)
            {
                throw new InvalidOperationException(
                    "No service locator known. Make sure your container adapter is a service locator, meaning it implements Microsoft.Practices.ServiceLocation.IServiceLocator");
            }

            _applicationContextResolverResolver = c => _serviceLocator.GetInstance<IApplicationContextResolver>();

            return this;
        }        
    }
}