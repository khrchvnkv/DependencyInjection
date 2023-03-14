using System.Collections.Concurrent;
using System.Reflection;

namespace DependencyInjection;

public interface IContainer
{
    IScope CreateScope();
}
public interface IScope
{
    object Resolve(Type service);
}
public class Container : IContainer
{
    private class Scope : IScope
    {
        private readonly Container _container;
        private readonly Dictionary<Type, object> _scopedInstances = new Dictionary<Type, object>();

        public Scope(Container container)
        {
            _container = container;
        }
        public object Resolve(Type service)
        {
            var descriptor = _container.FindDescriptor(service);
            if (descriptor.Lifetime == Lifetime.Transient)
            {
                _container.CreateInstance(service, this);
            }

            if (descriptor.Lifetime == Lifetime.Scoped || _container._rootScope == this)
            {
                if (_scopedInstances.TryGetValue(service, out var scopeInstance))
                {
                    return scopeInstance;
                }
                else
                {
                    var instance = _container.CreateInstance(service, this);
                    _scopedInstances.Add(service, instance);
                    return instance;
                }
            }
            else
            {
                return _container._rootScope.Resolve(service);
            }
        }
    }

    private readonly Dictionary<Type, ServiceDescriptor> _descriptors;
    private readonly Dictionary<Type, Func<IScope, object>> _buildActivators =
        new Dictionary<Type, Func<IScope, object>>();
    private readonly Scope _rootScope;

    public Container(IEnumerable<ServiceDescriptor> descriptors)
    {
        _descriptors = descriptors.ToDictionary(x => x.ServiceType);
        _rootScope = new Scope(this);
    }
    public IScope CreateScope()
    {
        return new Scope(this);
    }
    private Func<IScope, object> BuildActivation(Type service)
    {
        if (!_descriptors.TryGetValue(service, out var descriptor))
            throw new InvalidOperationException($"Service {service} is not registered");

        if (descriptor is InstanceBasedServiceDescriptor instanceBasedServiceDescriptor)
        {
            return _ => instanceBasedServiceDescriptor.Instance;
        }
        if (descriptor is FactoryBasedServiceDescriptor factoryBasedServiceDescriptor)
        {
            return factoryBasedServiceDescriptor.Factory;
        }
        
        var typeBasedDescriptor = (TypeBasedServiceDescriptor)descriptor;
        
        // Get Constructor
        var ctor = typeBasedDescriptor.ImplementationType.GetConstructors(BindingFlags.Public | 
                                                                          BindingFlags.Instance).Single();
        
        // Get Constructor args
        var args = ctor.GetParameters();

        return s =>
        {
            // Create args
            var argsForCtor = new object[args.Length];
            for (int i = 0; i < argsForCtor.Length; i++)
            {
                argsForCtor[i] = CreateInstance(args[i].ParameterType, s);
            }

            // Invoke constructor
            return ctor.Invoke(argsForCtor);
        };
    }
    private object CreateInstance(Type service, IScope scope)
    {
        if (_buildActivators.TryGetValue(service, out var activator))
        {
            return activator(scope);
        }
        else
        {
            var activation = BuildActivation(service);
            _buildActivators.Add(service, activation);
            return activation(scope);
        }
    }
    private ServiceDescriptor? FindDescriptor(Type service)
    {
        _descriptors.TryGetValue(service, out var descriptor);
        return descriptor;
    }
}