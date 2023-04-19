using System;
using System.Collections.ObjectModel;
using Guard.DynamicProxy.Abstracts.Interfaces;
using Guard.DynamicProxy.DependencyInjection.Interfaces;

namespace Guard.DynamicProxy.DependencyInjection {
    public class InterceptorCollection : Collection<ServiceInterceptorAttribute> {

        public ServiceInterceptorAttribute AddService<TInterceptorType>() where TInterceptorType : IInterceptor {
            return AddService(typeof(TInterceptorType));
        }

        public ServiceInterceptorAttribute AddService(Type interceptorType) {
            if (interceptorType == null) {
                throw new ArgumentNullException(nameof(interceptorType));
            }

            return AddService(interceptorType, order: 0);
        }

        public ServiceInterceptorAttribute AddService<TInterceptorType>(int order) where TInterceptorType : IInterceptor {
            return AddService(typeof(TInterceptorType), order);
        }

        public ServiceInterceptorAttribute AddService(Type interceptorType, int order) {
            if (interceptorType == null) {
                throw new ArgumentNullException(nameof(interceptorType));
            }

            if (!typeof(IInterceptor).IsAssignableFrom(interceptorType)) {
                throw new ArgumentException("interceptorType must inherit IInterceptor", nameof(interceptorType));
            }

            var filter = new ServiceInterceptorAttribute(interceptorType)
            { Order = order };
            Add(filter);
            return filter;
        }
    }
}

