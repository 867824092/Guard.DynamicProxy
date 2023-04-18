using BenchmarkDotNet.Running;

namespace Guard.DynamicProxy.Benchmarks {
    public class Program {
        static void Main(string[] args) {
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}

