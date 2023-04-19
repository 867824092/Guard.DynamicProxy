using System;
using Guard.DynamicProxy.Abstracts.Interfaces;

namespace Guard.DynamicProxy.Abstracts.Attributes {
    /// <summary>
    /// 拦截器特性
    /// </summary>
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class InterceptorAttribute : Attribute,IInterceptor {
        public abstract void Intercept(IInvocation invocation);
    }
} 

