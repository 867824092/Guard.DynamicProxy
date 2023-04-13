using System;

namespace Guard.DynamicProxy.Abstracts.Interfaces {
    /// <summary>
    /// Assemble Proxy Objects
    /// </summary>
    public interface IProxyBuilder {
        /// <summary>
        /// 创建类代理类型
        /// </summary>
        Type CreateClassProxyType<T>();
        /// <summary>
        /// 创建类代理类型
        /// </summary>
        Type CreateClassProxyType(Type targetType);
    }
}
