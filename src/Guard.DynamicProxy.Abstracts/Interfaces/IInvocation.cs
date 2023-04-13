using System;
using System.Reflection;

namespace Guard.DynamicProxy.Abstracts.Interfaces {
    /// <summary>
    /// 代理对象调用者
    /// </summary>
    public interface IInvocation {
        /// <summary>
        /// 拦截器
        /// </summary>
        IInterceptor[] Interceptors { get; }
        /// <summary>
        /// 方法参数数组
        /// </summary>
        object[] Arguments { get; }
        /// <summary>
        /// 泛型参数类型
        /// </summary>
        Type[] GenericTypeArguments { get; }
        /// <summary>
        /// 代理方法
        /// </summary>
        MethodInfo Method { get; }
        /// <summary>
        /// 原始方法
        /// </summary>
		MethodInfo MethodInvocationTarget { get; }
        /// <summary>
        /// 代理对象
        /// </summary>
        object Proxy { get; }
        /// <summary>
        /// 返回值
        /// </summary>
        object ReturnValue { get; set; }
        /// <summary>
        /// 目标对象
        /// </summary>
        Type TargetType { get; }

       /// <summary>
       /// 根据索引获取参数值
       /// </summary>
       object GetArgumentValue(int index);
        /// <summary>
        /// 继续调用行中的下一个拦截器，并最终调用目标方法。
        /// </summary>
        /// <remarks>
        /// 通过继承组合的方式，以俄罗斯套娃模式执行，如果没有拦截器，那么就会调用目标方法。
        /// </remarks>
        void Proceed();
    }
}

