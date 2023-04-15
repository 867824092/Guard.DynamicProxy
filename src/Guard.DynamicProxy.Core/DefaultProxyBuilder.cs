using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Guard.DynamicProxy.Abstracts;
using Guard.DynamicProxy.Abstracts.Interfaces;
using Guard.DynamicProxy.Core.Interal;

namespace Guard.DynamicProxy.Core {
    public class DefaultProxyBuilder : IProxyBuilder {
        private readonly ModuleScope _moduleScope;

        public DefaultProxyBuilder(ModuleScope moduleScope) {
            _moduleScope = moduleScope;
        }

        public Type CreateClassProxyType<T>() {
            return CreateClassProxyType(typeof(T));
        }
        
        public Type CreateClassProxyType(Type targetType) {
            var generator = new ClassProxyGenerator(targetType,_moduleScope);
            return generator.GetProxyType();
        }
        
        public Type CreateClassProxyType<T>(T target) {
            return CreateClassProxyType(typeof(T),target);
        }

        public Type CreateClassProxyType(Type targetType, object target) {
            if (targetType != target.GetType()) {
                throw new ArgumentException("targetType must be equal to target.GetType()");
            }

            var generator = new ClassProxyGeneratorWithTarget(targetType, target, _moduleScope);
            return generator.GetProxyType();
        }
    }
}

