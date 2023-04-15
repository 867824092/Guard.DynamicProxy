using System;

namespace Guard.DynamicProxy.Abstracts.Interfaces {
    /// <summary>
    /// Assemble Proxy Objects
    /// </summary>
    public interface IProxyBuilder {
        /// <summary>
        /// 创建代理类型
        /// </summary>
        Type CreateClassProxyType<T>();
        /// <summary>
        /// 创建代理类型
        /// </summary>
        Type CreateClassProxyType(Type targetType);
        /// <summary>
        /// 创建代理类型，由目标对象调用最终方法
        /// </summary>
        Type CreateClassProxyType<T>(T target);
        /// <summary>
        /// 创建代理类型，由目标对象调用最终方法
        /// </summary>
        Type CreateClassProxyType(Type targetType, object target);
    }
}
