using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Guard.DynamicProxy.Abstracts.Attributes;
using Guard.DynamicProxy.Abstracts.Interfaces;
using Guard.DynamicProxy.Core;
using Guard.DynamicProxy.DependencyInjection.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Guard.DynamicProxy.DependencyInjection {
    public static class ServiceCollectionExtensions {
        
        /// <summary>
        /// 对标记拦截特性的类或者接口进行注入
        /// </summary>
        public static IServiceCollection AddDynamicProxy(this IServiceCollection services,
            params Assembly[] assemblies) {
            return  services.AddDynamicProxy(null, assemblies);
        }
        
        /// <summary>
        /// 对标记拦截特性的类或者接口进行注入
        /// </summary>
        public static IServiceCollection AddDynamicProxy(this IServiceCollection services,
            Action<DynamicProxyOptions> configAction,
            params Assembly[] assemblies) {
            var options = new DynamicProxyOptions();
            configAction?.Invoke(options);
            services.Configure(configAction);
            services.AddDynamicProxyCore();
            if (assemblies != null) {
                services.ReplaceService(options, assemblies);
            }

            return services;
        }

        private static void ReplaceService(this IServiceCollection services, DynamicProxyOptions options,
            Assembly[] assemblies) {
             foreach (Assembly assembly in assemblies) {
                IEnumerable<Type> types = assembly.GetTypes().Where(u =>
                    u.IsPublic &&
                    !u.IsInterface &&
                    !u.IsAbstract
                    && u.GetInterface(nameof(IDynamicProxy)) != null);
                foreach (Type type in types) {
                    ServiceDescriptor serviceDescriptor =
                        services.FirstOrDefault(u => u.ImplementationType == type);
                    if (serviceDescriptor == null) continue;
                    
                    services.Replace(ServiceDescriptor.Describe(serviceDescriptor.ServiceType,
                        provider => {
                            //查找类上的拦截器特性
                            var typeInterceptors = type.GetCustomAttributes<InterceptorAttribute>().ToList();
                            var interceptors =
                                new IInterceptor[options.GlobalInterceptorCount + typeInterceptors.Count];
                            int index = 0;
                            foreach (ServiceInterceptorAttribute interceptor in options.Interceptors.OrderBy(
                                         u => u.Order)) {
                                interceptors[index++] = interceptor.CreateInstance(provider);
                            }

                            for (int i = 0; i < typeInterceptors.Count; i++) {
                                interceptors[i + options.GlobalInterceptorCount] = typeInterceptors[i];
                            }

                            object target = null;
                            if (serviceDescriptor.ImplementationInstance != null) {
                                target = serviceDescriptor.ImplementationInstance;
                            } else
                                if (serviceDescriptor.ImplementationFactory != null) {
                                    target = serviceDescriptor.ImplementationFactory(provider);
                                } else {
                                    target = ActivatorUtilities.CreateInstance(provider, type);
                                }

                            var proxyGenerator = provider.GetRequiredService<IProxyGenerator>();
                            var proxy = proxyGenerator.CreateClassProxyWithTarget(
                                serviceDescriptor.ImplementationType,
                                target,
                                interceptors);
                            return proxy;
                        }, serviceDescriptor.Lifetime));
                }
             }
        }
        
        private static IServiceCollection AddDynamicProxyCore(this IServiceCollection services) {
            services.AddSingleton<ModuleScope>();
            services.AddSingleton<IProxyBuilder, DefaultProxyBuilder>();
            services.AddSingleton<IProxyGenerator, DefaultProxyGenerator>();
            return services;
        }
    }
}

