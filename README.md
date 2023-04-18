# Guard.DynamicProxy
A Lightweight Class Library for Generating Dynamic Proxy Objects Based on Emit.

#### 1、Search for DynamicProxy. Core for installation through the nuget package.

#### 2、Example

```c#
public class Calculate : ICalculate {
   public virtual int Add(int a, int b) {
       return a + b;
   }
}
public interface ICalculate {
        int Add(int a, int b);
}
public class LogInterceptor : IInterceptor  {
   public void Intercept(IInvocation invocation) {
       Debug.WriteLine("LogInterceptor pre....");
       invocation.Proceed();
       Debug.WriteLine("LogInterceptor post....");
   }
}
var builder = new DefaultProxyBuilder(new ModuleScope());
var proxyGenerator = new DefaultProxyGenerator(builder);
//Using a public parameterless constructor
var calculator = proxyGenerator.CreateInterfaceProxy<ICalculate,Calculate>(new LogInterceptor());
//or Specify Constructor
calculator = proxyGenerator.CreateInterfaceProxy<ICalculate,Calculate>(new object[]{"test"},new LogInterceptor());
//or with target 
calculator = proxyGenerator.CreateInterfaceProxyWithTarget<ICalculate,Calculate>(new Calculate(),new LogInterceptor());
calculator.Add(1, 3);

//Ignore a method's proxy
//Add the Ignore attribute on the method
[Ignore]
public  int Multiply(int a, int b) {
    return a * b;
}
```

TODO

- [ ] Dependency Injection
