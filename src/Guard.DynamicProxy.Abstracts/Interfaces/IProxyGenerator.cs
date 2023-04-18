using System;

namespace Guard.DynamicProxy.Abstracts.Interfaces {
    /// <summary>
    /// Calling Proxy Object Creator
    /// </summary>
    public interface IProxyGenerator {
        #region WithOutTarget

        /// <summary>
        /// 创建代理类
        /// </summary>
        /// <param name="interceptors">拦截器</param>
        /// <typeparam name="TClass">引用类型</typeparam>
        TClass CreateClassProxy<TClass>(params IInterceptor[] interceptors) where TClass : class;

        /// <summary>
        /// 创建代理类
        /// </summary>
        /// <param name="constructorArguments">构造函数参数</param>
        /// <param name="interceptors">拦截器</param>
        /// <typeparam name="TClass">引用类型</typeparam>
        TClass CreateClassProxy<TClass>(object[] constructorArguments, params IInterceptor[] interceptors)
            where TClass : class;

        /// <summary>
        /// 创建接口代理
        /// </summary>
        /// <remarks>实现类要继承接口，必须在代码中声明。</remarks>
        /// <param name="interceptors">拦截器</param>
        /// <param name="targetType">实现该接口的类型</param>
        TInterface CreateInterfaceProxy<TInterface>(Type targetType, params IInterceptor[] interceptors);

        /// <summary>
        /// 创建接口代理
        /// </summary>
        /// <remarks>实现类要继承接口，必须在代码中声明。</remarks>
        /// <param name="targetType">实现该接口的类型</param>
        /// <param name="constructorArguments">实现该接口的类型的构造函数</param>
        /// <param name="interceptors">拦截器</param>
        TInterface CreateInterfaceProxy<TInterface>(Type targetType, object[] constructorArguments,
            params IInterceptor[] interceptors);

        /// <summary>
        /// 创建接口代理
        /// </summary>
        /// <remarks>实现类要继承接口，必须在代码中声明。</remarks>
        /// <param name="interceptors">拦截器</param>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <typeparam name="TClass">实现该接口的类型</typeparam>
        TInterface CreateInterfaceProxy<TInterface, TClass>(params IInterceptor[] interceptors) where TClass : class;

        /// <summary>
        /// 创建接口代理
        /// </summary>
        /// <remarks>实现类要继承接口，必须在代码中声明。</remarks>
        /// <param name="constructorArguments">声明类的构造函数参数</param>
        /// <param name="interceptors">拦截器</param>
        /// <typeparam name="TInterface">接口类型</typeparam>
        /// <typeparam name="TClass">实现类型</typeparam>
        TInterface CreateInterfaceProxy<TInterface, TClass>(object[] constructorArguments,
            params IInterceptor[] interceptors) where TClass : class;

        /// <summary>
        /// 创建代理类
        /// </summary>
        /// <param name="targetType">类型</param>
        /// <param name="interceptors">拦截器</param>
        /// <returns></returns>
        object CreateClassProxy(Type targetType, params IInterceptor[] interceptors);

        /// <summary>
        /// 创建代理类
        /// </summary>
        /// <param name="targetType">类型</param>
        /// <param name="constructorArguments">构造函数参数</param>
        /// <param name="interceptors">拦截器</param>
        object CreateClassProxy(Type targetType, object[] constructorArguments, params IInterceptor[] interceptors);

        #endregion

        #region WithTarget

        /// <summary>
        /// 根据目标对象创建代理类
        /// </summary>
        TClass CreateClassProxyWithTarget<TClass>(TClass target, params IInterceptor[] interceptors)
            where TClass : class;
        /// <summary>
        /// 根据目标对象创建代理类
        /// </summary>
        TInterface CreateInterfaceProxyWithTarget<TInterface, TClass>(TClass target,
            params IInterceptor[] interceptors) where TClass : class;
        /// <summary>
        /// 根据目标对象创建代理类
        /// </summary>
        TInterface CreateInterfaceProxyWithTarget<TInterface>(Type targetType, object target,
            params IInterceptor[] interceptors);
        /// <summary>
        /// 根据目标对象创建代理类
        /// </summary>
        object CreateInterfaceProxyWithTarget(Type interfaceType, Type targetType, object target,
            params IInterceptor[] interceptors);
        /// <summary>
        /// 根据目标对象创建代理类
        /// </summary>
        object CreateClassProxyWithTarget(Type targetType, object target, params IInterceptor[] interceptors);

        #endregion
    }
}
