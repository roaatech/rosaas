using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Attributes;
using Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Processors.Abstraction;
using Roaa.Rosas.Domain.Entities.Management;
using System.Reflection;

namespace Roaa.Rosas.Application.Services.Management.SubscriptionRenewals.Factories.Concretes
{
    public class SubscriptionRenewalFactory : ISubscriptionRenewalFactory
    {

        private readonly Dictionary<SubscriptionRenewalTypeEnum, Type> _processorsDictionary;
        private readonly IServiceProvider _serviceProvider;


        public SubscriptionRenewalFactory(IServiceProvider serviceProvider)
        {
            _processorsDictionary = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(type => typeof(SubscriptionRenewalProcessor).IsAssignableFrom(type) &&
                               type.GetCustomAttributes<SubscriptionRenewalTypeAttribute>(false).Any())
                .ToDictionary(type => type.GetCustomAttributes<SubscriptionRenewalTypeAttribute>(false).First().RenewalType, type => type);
            _serviceProvider = serviceProvider;
        }




        public SubscriptionRenewalProcessor InstantiateProcessor(SubscriptionRenewalTypeEnum renewalType)
        {
            if (_processorsDictionary.TryGetValue(renewalType, out Type? processorType))
            {
                var subscriptionRenewalProcessor = (SubscriptionRenewalProcessor)_serviceProvider.GetRequiredService(processorType);
                ArgumentNullException.ThrowIfNull(subscriptionRenewalProcessor);
                return subscriptionRenewalProcessor;
            }
            else
            {
                throw new NotImplementedException(@$"The  {nameof(SubscriptionRenewalFactory)} failed in instantiating {nameof(SubscriptionRenewalProcessor)}. 
                                                      can not create a new instance of the {nameof(SubscriptionRenewalProcessor)},
                                                       So you must implement a subscription renewal processor specific to ({renewalType}) renewal type");
            }
        }
    }

}
