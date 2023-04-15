using System;
using System.Reflection;
using Guard.DynamicProxy.Abstracts.Interfaces;

namespace Guard.DynamicProxy.Core.Invocation {
    /// <summary>
    /// 继承使用目标对象调用
    /// </summary>
    public class InheritWithTargetInvocation : AbstractInvocation {
        public object Target { get; }

        public InheritWithTargetInvocation(object[] arguments,
            object target,
            object proxy,
            Type targetType,
            IInterceptor[] interceptors,
            Type[] genericTypeArguments,
            MethodInfo method,
            MethodInfo methodInvocationTarget) : base(arguments, proxy, targetType, interceptors, genericTypeArguments,
            method, methodInvocationTarget) {
            Target = target;
        }
        protected override void InvocationWithTarget() {
            ReturnValue = !MethodInvocationTarget.IsGenericMethod 
                ? MethodInvocationTarget.Invoke(Target, Arguments) 
                : MethodInvocationTarget.MakeGenericMethod(GenericTypeArguments).Invoke(Target, Arguments);
        }
    }
}

