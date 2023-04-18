namespace Guard.DynamicProxy.Tests {
    public class Calculate : ICalculate {
        [MethodLogInterceptor]
        public virtual int Add(int a, int b) {
            return a + b;
        }
    }
    
    public interface ICalculate {
        int Add(int a, int b);
    }
}

