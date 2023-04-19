using Guard.DynamicProxy.Abstracts.Attributes;

namespace Guard.DynamicProxy.Tests {
    public class Calculate : ICalculate {
        public virtual int Add(int a, int b) {
            return a + b;
        }
        [Ignore]
        public  int Multiply(int a, int b) {
            return a * b;
        }
    }
    
    public interface ICalculate {
        int Add(int a, int b);
    }
}

