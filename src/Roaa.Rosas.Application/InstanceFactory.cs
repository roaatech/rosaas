using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Application.Attributes;
using System.Reflection;


namespace Roaa.Rosas.Application
{
    public class InstanceFactory<TAbstractParent, TAttribute> where TAbstractParent : class where TAttribute : BaseCustomAttribute
    {

        private readonly Dictionary<string, Type> _processorsDictionary;
        private readonly IServiceProvider _serviceProvider;


        public InstanceFactory(IServiceProvider serviceProvider)
        {
            _processorsDictionary = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(type => typeof(TAbstractParent).IsAssignableFrom(type) &&
                               type.GetCustomAttributes<TAttribute>(false).Any())
                .ToDictionary(type => type.GetCustomAttributes<TAttribute>(false).First().TypeAsString, type => type);
            _serviceProvider = serviceProvider;
        }




        public TAbstractParent InstantiateProcessor(string type)
        {
            if (_processorsDictionary.TryGetValue(type, out Type? processorType))
            {
                var subscriptionRenewalProcessor = (TAbstractParent)_serviceProvider.GetRequiredService(processorType);
                ArgumentNullException.ThrowIfNull(subscriptionRenewalProcessor);
                return subscriptionRenewalProcessor;
            }
            else
            {
                throw new NotImplementedException(@$"The  {GetType()} failed in instantiating {typeof(TAbstractParent)}. 
                                                      can not create a new instance of the {typeof(TAttribute)},
                                                       So you must implement a service specific to ({type}) type.");
            }
        }
    }
}