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
    
}
