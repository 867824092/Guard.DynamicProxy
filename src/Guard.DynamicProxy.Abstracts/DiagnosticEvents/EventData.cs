using System;

namespace Guard.DynamicProxy.Abstracts.DiagnosticEvents {
    public abstract class EventData {
        /// <summary>
        /// 事件命名空间
        /// </summary>
        protected const string EventNamespace = "Guard.DynamicProxy.";
        /// <summary>
        /// 真实对象类型
        /// </summary>
        public Type TargetType { get; }
        /// <summary>
        /// 代理对象类型
        /// </summary>
        public Type ProxyType { get; }
    }
}

