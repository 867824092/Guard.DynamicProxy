using System;
using Guard.DynamicProxy.Abstracts.Interfaces;

namespace Guard.DynamicProxy.DependencyInjection {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false,Inherited = true)]
    public abstract class AbstractInterceptorAttribute : Attribute, IInterceptor {
        /// <summary>
        /// 排序
        /// </summary>
        public uint Order { get; set; }
        protected AbstractInterceptorAttribute(uint order = 0) {
            Order = order;
        }
        public abstract void Intercept(IInvocation invocation);
    }
}

