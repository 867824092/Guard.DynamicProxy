using System.Collections.Generic;
using Guard.DynamicProxy.Abstracts.Interfaces;

namespace Guard.DynamicProxy.DependencyInjection {
    public sealed class DynamicProxyOptions {
        /// <summary>
        /// 全局拦截器
        /// </summary>
        public InterceptorCollection Interceptors { get; }
      
        public int GlobalInterceptorCount => Interceptors.Count;
        public DynamicProxyOptions() {
            Interceptors = new  InterceptorCollection();
        }
    }
} 

