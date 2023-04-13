using System;
using System.Reflection;
using Guard.DynamicProxy.Abstracts;
using Guard.DynamicProxy.Abstracts.Interfaces;

namespace Guard.DynamicProxy.Core {
    public abstract class AbstractInvocation : IInvocation {
        private int _interceptorIndex = -1;
        public object[] Arguments { get; }
        public MethodInfo MethodInvocationTarget { get; }
        public object Proxy { get; }
        public object ReturnValue { get; set; }
        public Type TargetType { get; }
        public IInterceptor[] Interceptors { get; }
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

