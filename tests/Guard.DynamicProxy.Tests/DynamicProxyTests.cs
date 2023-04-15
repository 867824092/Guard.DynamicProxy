using System;
using System.Reflection;
using Guard.DynamicProxy.Abstracts.Interfaces;
using Guard.DynamicProxy.Core;
using Xunit;

namespace Guard.DynamicProxy.Tests {
    public class DynamicProxyTests {
        private IProxyGenerator ProxyGenerator { get; }

        public DynamicProxyTests() {
            var builder = new DefaultProxyBuilder(new ModuleScope());
            ProxyGenerator = new DefaultProxyGenerator(builder);
        }
    
        [Fact]
        public void Should_True_Create_Wushen_Proxy() {
            var calculator = ProxyGenerator.CreateClassProxy<Calculate>(new LogInterceptor());
            Assert.True( calculator.Add(1, 3) == 4);
        }
        [Fact]
        public void Should_True_Create_Proxy_With_Target() {
            //TODO target为null
            var calculator = (Calculate)ProxyGenerator.CreateClassProxy(typeof(Calculate),new Calculate(), new LogInterceptor());
            Assert.True(calculator != null); 
            Assert.True( calculator.Add(1,6) == 7);
        }

        [Fact]
        public void Should_True_Create_Interface_Proxy() {
            ICalculate calculate = ProxyGenerator.CreateInterfaceProxy<ICalculate,Calculate>(new LogInterceptor());
            Assert.True(calculate.Add(1, 5) == 6);
            Assert.Throws<InvalidCastException>(() => {
                ProxyGenerator.CreateInterfaceProxy<IEmpty, Calculate>();
            });
            Assert.False(calculate is IEmpty);
        }
    }

    public interface IEmpty { }
}

