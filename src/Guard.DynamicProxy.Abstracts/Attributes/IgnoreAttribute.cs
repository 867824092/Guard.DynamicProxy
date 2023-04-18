using System;

namespace Guard.DynamicProxy.Abstracts.Attributes {
    /// <summary>
    /// 忽视代理此方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class IgnoreAttribute : Attribute {
    
    }
}

