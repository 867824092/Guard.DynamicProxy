using Guard.DynamicProxy.Abstracts.Interfaces;
using Guard.DynamicProxy.Core;
using Xunit;

namespace Guard.DynamicProxy.Tests {
    public class DynamicProxyTests {
        private IProxyGenerator ProxyGenerator { get; }

        public DynamicProxyTests() {
            var builder = new DefaultProxyBuilder(new ModuleScope(), null);
            ProxyGenerator = new DefaultProxyGenerator(builder);
        }
    
        [Fact]
        public void Should_True_Create_Wushen_Proxy() {
            var calculator = ProxyGenerator.CreateClassProxy<Calculate>(new LogInterceptor());
            Assert.True( calculator.Add(1, 3) == 4);
        }
    }
}

