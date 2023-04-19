using Guard.DynamicProxy.Abstracts.Attributes;
using Guard.DynamicProxy.Abstracts.Interfaces;
using Guard.DynamicProxy.DependencyInjection.Interfaces;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Guard.DynamicProxy.DependencyInjection.Tests; 

public class Calculate : ICalculate {
    readonly CalculateProvider _calculateProvider;
    public Calculate(CalculateProvider calculateProvider) {
        _calculateProvider = calculateProvider;
    }
    public virtual int Add(int a, int b) {
        return a + b;
    }
}
    
public interface ICalculate : IDynamicProxy {
    int Add(int a, int b);
}

public class CalculateProvider {
    
}
public class LogInterceptor : InterceptorAttribute {
    readonly ITestOutputHelper _output;
    public LogInterceptor(ITestOutputHelper output) {
        _output = output;
    }

    public override void Intercept(IInvocation invocation) {
        _output?.WriteLine("LogInterceptor before......");
        invocation.Proceed();
        _output?.WriteLine("LogInterceptor after......");
    }
}
