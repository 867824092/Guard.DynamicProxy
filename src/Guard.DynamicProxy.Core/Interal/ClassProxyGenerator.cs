using System;
using System.Reflection;
using System.Reflection.Emit;
using Guard.DynamicProxy.Abstracts.Interfaces;
using Guard.DynamicProxy.Core.Core;

namespace Guard.DynamicProxy.Core.Interal {
    /// <summary>
    /// 类代理生成器
    /// </summary>
    internal class ClassProxyGenerator : AbstractProxyGenerator {
        
        public ClassProxyGenerator(Type targetType, ModuleScope moduleScope):base(targetType,moduleScope) {
        }
        public ClassProxyGenerator(Type targetType, ModuleScope moduleScope,TypeAttributes flags = DefaultAttributes)
            :base(targetType,moduleScope,flags) {
        }
        protected override Type GenerateProxyType(Type targetType) {
            TypeBuilder typeBuilder = CreateTypeBuilder(targetType.Name);
            var emitter = new ClassEmitterBuilder(typeBuilder, targetType);

            emitter.AddPrivateFields(AbstractEmitterBuilder.InterceptorsFieldName, typeof(IInterceptor[])) // 生成拦截器字段
                .AddPrivateFields(AbstractEmitterBuilder.TargetTypeFieldName, typeof(Type)) // 生成targetType字段
                .AddConstructors()
                .AddMethods();

            return  emitter.Build();
        }
    }
}