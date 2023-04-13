using System;
using System.Collections.Generic;
using System.Text;
using Guard.DynamicProxy.Abstracts;
using Guard.DynamicProxy.Abstracts.Interfaces;
using Guard.DynamicProxy.Core.Core;

namespace Guard.DynamicProxy.Core {
    
    public class DefaultProxyGenerator : IProxyGenerator {

        readonly IProxyBuilder _proxyBuilder;
        public DefaultProxyGenerator(IProxyBuilder proxyBuilder) {
            _proxyBuilder = proxyBuilder;
        }
        
        public TClass CreateClassProxy<TClass>(params IInterceptor[] interceptors) where TClass : class {
	        return (TClass)CreateClassProxy(typeof(TClass),null,interceptors);
        }
        
        public TClass CreateClassProxy<TClass>(object[] constructorArguments, params IInterceptor[] interceptors) where TClass : class {
	        return (TClass)CreateClassProxy(typeof(TClass),constructorArguments,interceptors);
		}

        public object CreateClassProxy(Type targetType, params IInterceptor[] interceptors) {
	        return CreateClassProxy(targetType, null, interceptors);
        }
        public object CreateClassProxy(Type targetType, object[] constructorArguments, params IInterceptor[] interceptors)
		{
			targetType.CheckIsClass(nameof(targetType));
            targetType.CheckNotGenericType(nameof(targetType));
            
			var proxyType = _proxyBuilder.CreateClassProxyType(targetType);
			var length = constructorArguments == null ? 0 : constructorArguments.Length;
			var arguments = new object[2 + length];
			arguments[0] = interceptors; 
			arguments[1] = targetType;
			if (constructorArguments != null && constructorArguments.Length > 0) {
				Array.Copy(constructorArguments, 0, arguments, 2, constructorArguments.Length);
			}
			return CreateClassProxyInstance(proxyType, arguments, targetType, constructorArguments);
		}

		protected object CreateClassProxyInstance(Type proxyType, object[] proxyArguments, Type targetType,
		                                          object[] constructorArguments)
		{
			try
			{
				return Activator.CreateInstance(proxyType, proxyArguments);
			}
			catch (MissingMethodException ex)
			{
				var message = new StringBuilder();
				message.AppendFormat("Can not instantiate proxy of class: {0}.", targetType.FullName);
				message.AppendLine();
				if (constructorArguments == null || constructorArguments.Length == 0)
				{
					message.Append("Could not find a parameterless constructor.");
				}
				else
				{
					message.AppendLine("Could not find a constructor that would match given arguments:");
					foreach (var argument in constructorArguments)
					{
						var argumentText = argument == null ? "<null>" : argument.GetType().ToString();
						message.AppendLine(argumentText);
					}
				}

				throw new ArgumentException(message.ToString(), nameof(constructorArguments), ex);
			}
		}
    }
} 
