namespace DependencyInjection;

public enum Lifetime
{
    Transient,
    Scoped,
    Singleton
}
public abstract class ServiceDescriptor
{
    public Type ServiceType { get; init; }
    public Lifetime Lifetime { get; init; }
}

public class TypeBasedServiceDescriptor : ServiceDescriptor
{
    // type
    public Type ImplementationType { get; init; }
}
public class FactoryBasedServiceDescriptor : ServiceDescriptor
{
    // delegate
    public Func<IScope, object> Factory { get; init; }
}

public class InstanceBasedServiceDescriptor : ServiceDescriptor
{
    // instance
    public object Instance { get; init; }

    public InstanceBasedServiceDescriptor(Type serviceType, object instance)
    {
        Lifetime = Lifetime.Singleton;
        ServiceType = serviceType;
        Instance = instance;
    }
}