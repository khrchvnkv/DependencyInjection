namespace DependencyInjection;

public interface IContainerBuilder
{
    void Register(ServiceDescriptor serviceDescriptor);
    IContainer Build();
}

public class ContainerBuilder : IContainerBuilder
{
    private readonly List<ServiceDescriptor> _descriptors = new List<ServiceDescriptor>();
    
    public void Register(ServiceDescriptor serviceDescriptor)
    {
        _descriptors.Add(serviceDescriptor);
    }
    public IContainer Build()
    {
        return new Container(_descriptors);
    }
}