using System;
using System.Reflection;
using Guard.DynamicProxy.Abstracts.Interfaces;

namespace Guard.DynamicProxy.Core.Invocation {
    /// <summary>
    ///  继承模式调用
    /// </summary>
    public class InheritInvocation : AbstractInvocation  {
        public InheritInvocation(object[] arguments, 
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

