namespace DependencyInjection;

public static class ContainerBuilderExtensions
{
    public static IContainerBuilder RegisterSingleton(this IContainerBuilder builder, Type serviceType,
        Type implementation) => builder.RegisterType(serviceType, implementation, Lifetime.Singleton);
    public static IContainerBuilder RegisterTransient(this IContainerBuilder builder, Type serviceType,
        Type implementation) => builder.RegisterType(serviceType, implementation, Lifetime.Transient);
    public static IContainerBuilder RegisterScoped(this IContainerBuilder builder, Type serviceType,
        Type implementation) => builder.RegisterType(serviceType, implementation, Lifetime.Scoped);
    public static IContainerBuilder RegisterSingleton<TService, TImplementation>(this IContainerBuilder builder) 
        => builder.RegisterType(typeof(TService), typeof(TImplementation), Lifetime.Singleton);
    public static IContainerBuilder RegisterTransient<TService, TImplementation>(this IContainerBuilder builder) 
        => builder.RegisterType(typeof(TService), typeof(TImplementation), Lifetime.Transient);
    public static IContainerBuilder RegisterScoped<TService, TImplementation>(this IContainerBuilder builder) 
        => builder.RegisterType(typeof(TService), typeof(TImplementation), Lifetime.Scoped);
    public static IContainerBuilder RegisterSingleton(this IContainerBuilder builder, Type serviceType,
        Func<IScope, object> factory) => builder.RegisterFactory(serviceType, factory, Lifetime.Singleton);
    public static IContainerBuilder RegisterTransient<TService>(this IContainerBuilder builder, Func<IScope, TService> factory) 
        => builder.RegisterFactory(typeof(TService), s => factory(s), Lifetime.Transient);
    public static IContainerBuilder RegisterScoped(this IContainerBuilder builder, Type serviceType,
        Func<IScope, object> factory) => builder.RegisterFactory(serviceType, factory, Lifetime.Scoped);
    public static IContainerBuilder RegisterSingleton<TService>(this IContainerBuilder builder, object instance) 
        => builder.RegisterInstance(typeof(TService), instance);
    
    private static IContainerBuilder RegisterType(this IContainerBuilder builder, Type serviceType, Type implementation,
        Lifetime lifetime)
    {
        builder.Register(new TypeBasedServiceDescriptor()
        {
            ImplementationType = implementation,
            ServiceType = serviceType,
            Lifetime = lifetime
        });
        return builder;
    }
    private static IContainerBuilder RegisterFactory(this IContainerBuilder builder, Type serviceType,
        Func<IScope, object> factory, Lifetime lifetime)
    {
        builder.Register(new FactoryBasedServiceDescriptor()
        {
            Factory = factory,
            ServiceType = serviceType,
            Lifetime = lifetime
        });
        return builder;
    }
    private static IContainerBuilder RegisterInstance(this IContainerBuilder builder, Type serviceType, object instance)
    {
        builder.Register(new InstanceBasedServiceDescriptor(serviceType, instance));
        return builder;
    }
}