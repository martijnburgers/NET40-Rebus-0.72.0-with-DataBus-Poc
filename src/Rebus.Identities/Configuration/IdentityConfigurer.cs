using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.IdentityModel.Claims;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using Rebus.Configuration;
using Rebus.IdentityClaims.Serialization;
using Rebus.IdentityClaims.Util;
using Rebus.Logging;

namespace Rebus.IdentityClaims.Configuration
{
    public class IdentityConfigurer : BaseConfigurer
    {
        public const string IdentityClaimsItemKey = "rebus-identity-claims";
        public const string IdentityClaimsHeaderKey = "rebus-identity-claims";
        private static ILog _log;

        private static ThreadIdentityClaimsResolver _threadIdentityClaimsResolver;

        private static ThreadIdentityClaimsResolver ThreadIdentityClaimsResolverInstance
        {
            get
            {
                return _threadIdentityClaimsResolver ??
                       (_threadIdentityClaimsResolver = new ThreadIdentityClaimsResolver());
            }
        }

        private Func<RebusIdentityResolverContext, IIdentityClaimsResolver> _identityClaimsResolverResolver =
            c => ThreadIdentityClaimsResolverInstance;
        
        private IServiceLocator _serviceLocator;
        private bool _transferClaims = true;

        static IdentityConfigurer()
        {
            RebusLoggerFactory.Changed += f => _log = f.GetCurrentClassLogger();
        }

        /// <summary>
        ///     CTOR
        /// </summary>
        /// <param name="backbone">The backbone of Rebus</param>
        public IdentityConfigurer(ConfigurationBackbone backbone) : base(backbone)
        {
            TryConfigureServiceLocator();
            ConfigureEvents();
        }

        protected internal IIdentityClaimsResolver IdentityClaimsResolver
        {
            get
            {
                return GuardForResolvingNulls(_identityClaimsResolverResolver(new RebusIdentityResolverContext(this)));
            }
        }

        private static T GuardForResolvingNulls<T>(T objectToCheck) where T : class
        {
            if (objectToCheck == null)
                throw new RebusIdentityConfigurationException(
                    string.Format(
                        "Could not resolve a type implementing '{0}'. You are probably missing some configuration somewhere.",
                        typeof (T).FullName));
            ;

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
                        if (_transferClaims)
                            AddClaimsToHeader(IdentityClaimsResolver, messageBus, message);
                    };
                });

            Backbone.ConfigureEvents(e => { e.MessageContextEstablished += AddClaimsToMessageContext; });
        }

        private static void AddClaimsToHeader(IIdentityClaimsResolver identityClaimsResolver, IBus bus, object message)
        {
            IEnumerable<Claim> claims = identityClaimsResolver.GetClaims();

            if (claims == null)
            {
                _log.Warn("Resolved claims from resolver type '{0}' are null.", identityClaimsResolver.GetType());

                return;
            }

            if (!claims.Any())
            {
                _log.Warn("Empty claims");
            }

            bus.AttachClaims(message, claims);
        }

        private static void AddClaimsToMessageContext(IBus bus, IMessageContext messageContext)
        {
            IDictionary<string, object> headers = messageContext.Headers;

            if (!headers.ContainsKey(IdentityClaimsHeaderKey))
            {
                _log.Warn("No claims found inside the message headers.");

                return;
            }

            string serializedClaims = (string) headers[IdentityClaimsHeaderKey];

            messageContext.Items[IdentityClaimsItemKey] =
                JsonConvert.DeserializeObject<IEnumerable<Claim>>(serializedClaims, new ClaimConverter());
        }

        protected internal void UseIdentityClaimsResolver(
            Func<RebusIdentityResolverContext, IIdentityClaimsResolver> factory)/*todo add param for indicating you want the result of the factory to be cached.*/
        {
            if (factory == null) throw new ArgumentNullException("factory");

            _identityClaimsResolverResolver = factory;
        }

        protected internal IdentityConfigurer UseServiceLocator()
        {
            if (_serviceLocator == null)
            {
                throw new InvalidOperationException(
                    "No service locator known. Make sure your container adapter is a service locator, meaning it implements Microsoft.Practices.ServiceLocation.IServiceLocator");
            }

            _identityClaimsResolverResolver = c => _serviceLocator.GetInstance<IIdentityClaimsResolver>();

            return this;
        }

        protected internal void ReceiveOnly()
        {
            _transferClaims = false;
        }
    }
}