using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Guard.DynamicProxy.Core.Interal {
    public abstract class AbstractProxyGenerator {
        protected const TypeAttributes DefaultAttributes =
            TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable;
        protected TypeAttributes TypeAttributes { get; }
        protected Type TargetType { get; }
        protected ModuleScope ModuleScope { get; }

        protected AbstractProxyGenerator(Type targetType,
            ModuleScope moduleScope,
            TypeAttributes typeAttributes = DefaultAttributes) {
            TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
            ModuleScope = moduleScope ?? throw new ArgumentNullException(nameof(moduleScope));
            TypeAttributes = typeAttributes;
        }
        
        public Type GetProxyType() {
            return ModuleScope.GetOrAdd(TargetType, GenerateProxyType);
        }
        protected TypeBuilder CreateTypeBuilder(string typeName) {
            return ModuleScope.CreateTypeBuilder(typeName, TypeAttributes, TargetType);
        }
        protected abstract Type GenerateProxyType(Type targetType);
    }
}

