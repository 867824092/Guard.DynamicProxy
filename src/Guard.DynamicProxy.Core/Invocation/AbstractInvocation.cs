using System;
using System.Linq;
using System.Reflection;
using Guard.DynamicProxy.Abstracts.Attributes;
using Guard.DynamicProxy.Abstracts.Interfaces;

namespace Guard.DynamicProxy.Core.Invocation {
    public abstract class AbstractInvocation : IInvocation {
        private int _interceptorIndex = -1;
        public object[] Arguments { get; }
        public MethodInfo MethodInvocationTarget { get; }
        public object Proxy { get; }
        public object ReturnValue { get; set; }
        public Type TargetType { get; }
        public IInterceptor[] Interceptors { get; private set; }
        public Type[] GenericTypeArguments { get; }
        public MethodInfo Method { get; }

        protected AbstractInvocation(object[] arguments,
            object proxy, 
            Type targetType, 
            IInterceptor[] interceptors, 
            Type[] genericTypeArguments, 
            MethodInfo method,
            MethodInfo methodInvocationTarget) { 
            Arguments = arguments;
            Proxy = proxy;
            TargetType = targetType;
            Interceptors = interceptors;
            GenericTypeArguments = genericTypeArguments;
            Method = method;
            MethodInvocationTarget = methodInvocationTarget;
        }
        public object GetArgumentValue(int index) {
            if(Arguments == null || Arguments.Length == 0) {
                throw new ArgumentNullException(nameof(Arguments));
            }
            if(index < 0 || index > Arguments.Length) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            return Arguments[index];
        }
        protected abstract void InvocationWithTarget();

        public void Proceed() {
            if (_interceptorIndex == -1) {
                //如果方法有标明拦截器特性，则将拦截器添加到拦截器列表中
                if (Method != null) {
                    var methodInterceptors =
                        Method.CustomAttributes.Where(u=>u.AttributeType.IsSubclassOf(typeof(InterceptorAttribute))).ToArray();
                    if (methodInterceptors != null && methodInterceptors.Length > 0) {
                        var interceptorList = new IInterceptor[Interceptors.Length + methodInterceptors.Length];
                        Interceptors.CopyTo(interceptorList, 0);
                        for (int i = 0; i < methodInterceptors.Length; i++) {
                            object[] objs = methodInterceptors[i].ConstructorArguments.Select(u => u.Value).ToArray();
                            var interceptor = (IInterceptor)Activator.CreateInstance(methodInterceptors[i].AttributeType,objs);
                            interceptorList[Interceptors.Length + i] = interceptor;
                        }
                        Interceptors = interceptorList;
                    }
                }
            }

            if (Interceptors == null || Interceptors.Length == 0) {
                InvocationWithTarget();
                return;
            }
            _interceptorIndex++;
            if (_interceptorIndex == Interceptors.Length) {
                InvocationWithTarget();
            } else
                if (_interceptorIndex > Interceptors.Length) {
                    throw new InvalidOperationException("No more interceptors to proceed to.");
                } else {
                    Interceptors[_interceptorIndex].Intercept(this);
                }
        }
    }
}

