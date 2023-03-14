using DependencyInjection;
using DependencyInjection.Services;

IContainerBuilder builder = new ContainerBuilder();
var container = builder
    .RegisterSingleton<ITestSingletonService, TestSingletonService>()
    .RegisterTransient<ITestTransientService>(s => new TestTransientService())
    .RegisterScoped<ITestScopedService, TestScopedService>()
    .Build();

var scope_1 = container.CreateScope();
var testScoped_1 = scope_1.Resolve(typeof(ITestScopedService));
var testScoped_2 = scope_1.Resolve(typeof(ITestScopedService));
if (testScoped_1 != testScoped_2)
{
    throw new InvalidOperationException();
}

var scope_2 = container.CreateScope();
var testScoped_3 = scope_2.Resolve(typeof(ITestScopedService));
if (testScoped_1 == testScoped_3)
{
    throw new InvalidOperationException();
}

var singleton_1 = scope_1.Resolve(typeof(ITestSingletonService));
var singleton_2 = scope_2.Resolve(typeof(ITestSingletonService));
if (singleton_1 != singleton_2)
{
    throw new InvalidOperationException();
}

Console.ReadLine();
