using System;
using System.Reflection;
using Guard.DynamicProxy.Abstracts;
using Guard.DynamicProxy.Abstracts.Interfaces;

namespace Guard.DynamicProxy.Core {
    /// <summary>
    ///  组合模式调用
    /// </summary>
    public class CompositionInvocation : AbstractInvocation  {
        public CompositionInvocation(object[] arguments, 
            object proxy, 
            Type targetType,
            IInterceptor[] interceptors, 
            Type[] genericTypeArguments, 
            MethodInfo method, 
            MethodInfo methodInvocationTarget) : 
            base(arguments, proxy, targetType, interceptors, genericTypeArguments, method, methodInvocationTarget) { }
        protected override void InvocationWithTarget() {
            ReturnValue = !MethodInvocationTarget.IsGenericMethod 
                ? MethodInvocationTarget.Invoke(Proxy, Arguments) 
                : MethodInvocationTarget.MakeGenericMethod(GenericTypeArguments).Invoke(Proxy, Arguments);
        }
    }
}

