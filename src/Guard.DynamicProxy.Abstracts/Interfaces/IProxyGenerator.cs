using System;

namespace Guard.DynamicProxy.Abstracts.Interfaces {
    /// <summary>
    /// Calling Proxy Object Creator
    /// </summary>
    public interface IProxyGenerator {
        TClass CreateClassProxy<TClass>(params IInterceptor[] interceptors) where TClass : class;
        TClass CreateClassProxy<TClass>(object[] constructorArguments, params IInterceptor[] interceptors)
            where TClass : class;
        TInterface CreateInterfaceProxy<TInterface>(Type targetType, params IInterceptor[] interceptors);
        TInterface CreateInterfaceProxy<TInterface>(Type targetType, object[] constructorArguments, params IInterceptor[] interceptors);
        TInterface CreateInterfaceProxy<TInterface, TClass>(params IInterceptor[] interceptors) where TClass : class;
        TInterface CreateInterfaceProxy<TInterface, TClass>(object[] constructorArguments, params IInterceptor[] interceptors) where TClass : class;
        object CreateClassProxy(Type targetType, params IInterceptor[] interceptors);
        object CreateClassProxy(Type targetType, object[] constructorArguments, params IInterceptor[] interceptors);
    }
}
