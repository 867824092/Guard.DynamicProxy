using System;
using System.Reflection;
using System.Reflection.Emit;
using Guard.DynamicProxy.Abstracts.Interfaces;
using Guard.DynamicProxy.Core.Core;

namespace Guard.DynamicProxy.Core.Interal {
    
    public class ClassProxyGeneratorWithTarget: AbstractProxyGenerator {
      
        /// <summary>
        /// 目标对象
        /// </summary>
        public object Target { get; }

        public ClassProxyGeneratorWithTarget(Type targetType, object target,
            ModuleScope moduleScope,
            TypeAttributes typeAttributes = DefaultAttributes) : base(targetType, moduleScope, typeAttributes) {
            Target = target;
        }
        protected override Type GenerateProxyType(Type targetType) {
            TypeBuilder typeBuilder =  CreateTypeBuilder(targetType.Name);
            var emitter = new ClassEmitterWithTargetBuilder(typeBuilder, targetType,Target);

            emitter.AddPrivateFields(AbstractEmitterBuilder.InterceptorsFieldName, typeof(IInterceptor[])) // 生成拦截器字段
                .AddPrivateFields(AbstractEmitterBuilder.TargetTypeFieldName, typeof(Type)) // 生成targetType字段
                .AddPrivateFields(ClassEmitterWithTargetBuilder.TargetFieldName, typeof(object)) // 生成target字段
                .AddConstructors(false)
                .AddMethods();

            return  emitter.Build();
        }
    }
}
