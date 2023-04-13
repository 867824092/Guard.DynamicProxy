using System;
using System.Reflection;
using System.Reflection.Emit;
using Guard.DynamicProxy.Abstracts.Interfaces;
using Guard.DynamicProxy.Core.Core;

namespace Guard.DynamicProxy.Core.Interal {
    /// <summary>
    /// 类代理生成器
    /// </summary>
    internal class ClassProxyGenerator {
        internal const TypeAttributes DefaultAttributes =
            TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable;
        public TypeAttributes TypeAttributes { get; }
        public Type TargetType { get; }
        public ModuleScope ModuleScope { get; }

        public ClassProxyGenerator(Type targetType, ModuleScope moduleScope,TypeAttributes typeAttributes = DefaultAttributes) {
            TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
            ModuleScope = moduleScope ?? throw new ArgumentNullException(nameof(moduleScope));
            TypeAttributes = typeAttributes;
        }
        public Type GetProxyType() {
            return ModuleScope.GetOrAdd(TargetType, GenerateProxyType);
        }
        private Type GenerateProxyType(Type targetType) {
            TypeBuilder typeBuilder =  ModuleScope.CreateTypeBuilder(targetType.Name,TypeAttributes,targetType,Type.EmptyTypes);
            var emitter = new ClassEmitterBuilder(typeBuilder, targetType);

            emitter.AddPrivateFields(ClassEmitterBuilder.InterceptorsFieldName, typeof(IInterceptor[])) // 生成拦截器字段
                .AddPrivateFields(ClassEmitterBuilder.TargetTypeFieldName, typeof(Type)) // 生成targetType字段
                .AddConstructors()
                .AddMethods();

            return  emitter.Build();
        }
    }
}