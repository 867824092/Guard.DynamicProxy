using System.Threading.Tasks;

namespace Guard.DynamicProxy.Abstracts.Interfaces {
    /// <summary>
    /// 方法需要拦截的拦截器
    /// </summary>
    public interface IInterceptor {
        /// <summary>
        /// Interception synchronization method
        /// </summary>
        void Intercept(IInvocation invocation);
    }
}
