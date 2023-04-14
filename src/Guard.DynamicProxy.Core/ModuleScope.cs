using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Guard.DynamicProxy.Core {
    public sealed class ModuleScope {
        public const string DefaultAssemblyName = "Guard.DynamicProxy";
        public const string DefaultModuleName = "Guard.DynamicProxy.Generated.dll";
        public const string DefaultProxyTypeName = "DynamicProxy.";
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly ConcurrentDictionary<Type, Type> _cache; 
        private readonly ModuleBuilder _moduleBuilder;
        public ModuleScope() {
            var assemblyName = new AssemblyName(DefaultAssemblyName);
            AssemblyBuilder assemblyBuilder;
            //.NetFramework 支持AppDomain
#if FEATURE_APPDOMAIN
			assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
					assemblyName, AssemblyBuilderAccess.RunAndSave);
#else
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
#endif
            _moduleBuilder = assemblyBuilder.DefineDynamicModule(DefaultModuleName);
            _cache = new ConcurrentDictionary<Type, Type>();
        }

        public Type GetOrAdd(Type key, Func<Type, Type> valueFactory) {
            //双检索判断
            if (_cache.TryGetValue(key, out var value)) {
                return value;
            }

            try {
                _semaphoreSlim.Wait();
                if (_cache.TryGetValue(key, out value)) {
                    return value;
                }

                value = valueFactory(key);
                _cache.TryAdd(key, value);
                return value;
            }
            finally {
                if (_semaphoreSlim.CurrentCount == 0) {
                    _semaphoreSlim.Release();
                }
            }

        }

        public TypeBuilder CreateTypeBuilder(string name, TypeAttributes attributes, Type baseType) {
            if (baseType != null && baseType.IsGenericTypeDefinition) {
                throw new NotSupportedException("不支持开放的泛型基类型: " + baseType.FullName);
            }            
            return _moduleBuilder.DefineType(DefaultProxyTypeName + name, attributes, baseType);
        }
    }
}