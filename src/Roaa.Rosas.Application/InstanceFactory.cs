using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Domain.Attributes;
using System.Reflection;


namespace Roaa.Rosas.Application
{
    public interface IInstanceFactory<TAbstractParent, TAttribute> where TAbstractParent : class where TAttribute : BaseCustomAttribute
    {
        TAbstractParent CreateInstanceFromScoped(string flagType);
        TAbstractParent CreateInstance(string type, params object?[]? args);
    }


    public class InstanceFactory<TAbstractParent, TAttribute> : IInstanceFactory<TAbstractParent, TAttribute> where TAbstractParent : class where TAttribute : BaseCustomAttribute
    {

        private readonly Dictionary<string, Type> _processorsDictionary;
        private readonly IServiceProvider _serviceProvider;


        public InstanceFactory(IServiceProvider serviceProvider)
        {
            var assembliesOfRoSaasProjects = AppDomain.CurrentDomain
                                                      .GetAssemblies()
                                                      .Where(x => x.FullName!.Contains("Roaa.Rosas") ||
                                                                  x.FullName.Contains("Roaa.Rosaas") ||
                                                                  x.FullName.Contains("Roaa.RoSaas") ||
                                                                  x.FullName.Contains("Roaa.RoSaaS"));

            _processorsDictionary = assembliesOfRoSaasProjects.SelectMany(x => x.ExportedTypes)
                                       .Where(type => typeof(TAbstractParent).IsAssignableFrom(type) &&
                                          type.GetCustomAttributes<TAttribute>(false).Any())
                .ToDictionary(type => type.GetCustomAttributes<TAttribute>(false).First().TypeAsString, type => type);
            _serviceProvider = serviceProvider;
        }




        public TAbstractParent CreateInstanceFromScoped(string type)
        {
            if (_processorsDictionary.TryGetValue(type, out Type? processorType))
            {
                var processor = (TAbstractParent)_serviceProvider.GetRequiredService(processorType);
                ArgumentNullException.ThrowIfNull(processor);
                return processor;
            }
            else
            {
                throw new NotImplementedException(@$"The  {GetType()} failed in instantiating {typeof(TAbstractParent)}. 
                                                      can not create a new instance of the {typeof(TAttribute)},
                                                       So you must implement a service specific to ({type}) type.");
            }
        }



        public TAbstractParent CreateInstance(string type, params object?[]? args)
        {
            if (_processorsDictionary.TryGetValue(type, out Type? processorType))
            {
                var instance = Activator.CreateInstance(processorType, args);

                var processor = instance as TAbstractParent;

                return processor!;
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