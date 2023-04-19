using System.Reflection;
using Guard.DynamicProxy.Abstracts.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Guard.DynamicProxy.DependencyInjection.Tests; 

public class DynamicDenpendencyInjectionTest {
    public IServiceProvider ServiceProvider { get; set; }
    public ITestOutputHelper OutputHelper { get; set; }

    public DynamicDenpendencyInjectionTest(ITestOutputHelper outputHelper) {
        OutputHelper = outputHelper;
        var services = new ServiceCollection();
        services.AddSingleton<CalculateProvider>();
        services.AddTransient<ICalculate, Calculate>();
        services.AddTransient(provider => new LogInterceptor(OutputHelper));
        services.AddDynamicProxy(options => {
            options.Interceptors.AddService(typeof(LogInterceptor));
        },Assembly.GetExecutingAssembly());
        ServiceProvider = services.BuildServiceProvider();
    }
    
    [Fact]
    public void Should_True_When_DynamicProxy() {
        var calculate = ServiceProvider.GetService<ICalculate>();
        Assert.Equal(3, calculate.Add(1, 2));
        
        Assert.NotEqual(calculate,ServiceProvider.GetService<ICalculate>());
    }
}