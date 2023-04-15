using System;
using System.Linq;
using System.Reflection;
using Guard.DynamicProxy.Abstracts.Interfaces;
using Guard.DynamicProxy.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Guard.DynamicProxy.DependencyInjection {
    public static class ServiceCollectionExtensions {
        //对标记拦截特性的类或者接口进行注入
        public static IServiceCollection AddDynamicProxy(this IServiceCollection services,params Assembly[] assemblies) {
            services.AddSingleton<ModuleScope>();
            services.AddSingleton<IProxyBuilder, DefaultProxyBuilder>();
            services.AddSingleton<IProxyGenerator, DefaultProxyGenerator>();
            return services;
        }
    }
}

