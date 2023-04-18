using System.Diagnostics;
using System.Threading.Tasks;
using Guard.DynamicProxy.Abstracts.Interfaces;

namespace Guard.DynamicProxy.Tests {
 
    public class LogInterceptor : IInterceptor  {
        public void Intercept(IInvocation invocation) {
            Debug.WriteLine("LogInterceptor pre....");
            invocation.Proceed();
            Debug.WriteLine("LogInterceptor post....");
        }
    }

    public class MethodLogInterceptorAttribute : Abstracts.Attributes.InterceptorAttribute {
        public override void Intercept(IInvocation invocation) {
            Debug.WriteLine("MethodLogInterceptor pre....");
            invocation.Proceed();
            Debug.WriteLine("MethodLogInterceptor post....");
        }
    }
    
}
