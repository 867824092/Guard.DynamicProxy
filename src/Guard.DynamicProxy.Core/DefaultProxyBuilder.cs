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
        private readonly DiagnosticListener _listener;
        
        public DefaultProxyBuilder(ModuleScope moduleScope, DiagnosticListener listener) {
            _moduleScope = moduleScope;
            _listener = listener;
        }

        public Type CreateClassProxyType<T>() {
            return CreateClassProxyType(typeof(T));
        }

        public Type CreateClassProxyType(Type targetType) {
            var generator = new ClassProxyGenerator(targetType,_moduleScope);
            return generator.GetProxyType();
        }

    }
}

