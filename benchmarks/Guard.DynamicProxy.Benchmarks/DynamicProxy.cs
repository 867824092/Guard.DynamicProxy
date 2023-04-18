using System.Collections.Generic;
using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Guard.DynamicProxy.Abstracts.Interfaces;
using Guard.DynamicProxy.Core;

namespace Guard.DynamicProxy.Benchmarks {
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net462)]
    [MemoryDiagnoser]
    public class DynamicProxy {
        private IProxyGenerator ProxyGenerator { get; set; }
        private Castle.DynamicProxy.IProxyGenerator CastleDynamicProxyProxyGenerator { get; set; }
    
        [GlobalSetup]
        public void Setup() {
            var builder = new DefaultProxyBuilder(new ModuleScope());
            ProxyGenerator = new DefaultProxyGenerator(builder);
            
            CastleDynamicProxyProxyGenerator = new Castle.DynamicProxy.ProxyGenerator();
        }

        [Benchmark]
        public void CreateClassProxyByGuardDynamicCore() {
            var calculator = ProxyGenerator.CreateClassProxy<Calculate>(new LogInterceptor());
            calculator.Add(1, 4);
        }
        [Benchmark]
        public void CreateClassProxyByCastleCoreDynamic() {
            var calculator = CastleDynamicProxyProxyGenerator.CreateClassProxy<Calculate>(new CastleLogInterceptor());
            calculator.Add(1, 4);
        }
    }
    public class LogInterceptor : IInterceptor  {
        public void Intercept(IInvocation invocation) {
            Debug.WriteLine("LogInterceptor pre....");
            invocation.Proceed();
            Debug.WriteLine("LogInterceptor post....");
        }
    }
    
    public class CastleLogInterceptor : Castle.DynamicProxy.IInterceptor {
        public void Intercept(Castle.DynamicProxy.IInvocation invocation) {
            Debug.WriteLine("LogInterceptor pre....");
            invocation.Proceed();
            Debug.WriteLine("LogInterceptor post....");
        }
    }
}

