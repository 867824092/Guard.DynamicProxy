using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Guard.DynamicProxy.Abstracts.Interfaces;
using Guard.DynamicProxy.Core.Interal;
using Guard.DynamicProxy.Core.Invocation;

namespace Guard.DynamicProxy.Core.Core {
    public class ClassEmitterBuilder : AbstractEmitterBuilder {
      
        public ClassEmitterBuilder(TypeBuilder typeBuilder, Type targetType) : base(typeBuilder, targetType) { }

        protected override void CreateConstructor() {
            for (int i = 0; i < ConstructorInfos.Count; i++) {
                ParameterInfo[] ctorParams = ConstructorInfos[i].GetParameters();
                Type[] parameterTypes = new Type[ctorParams.Length + 2];
                parameterTypes[0] = typeof(IInterceptor[]);
                parameterTypes[1] = typeof(Type);
                for (int j = 0; j < ctorParams.Length; j++) {
                    parameterTypes[j + 2] = ctorParams[j].ParameterType;
                }
                // 生成构造函数，添加额外的参数一个是拦截器，一个是targetType
                ConstructorBuilder constructorBuilder = TypeBuilder.DefineConstructor(
                    MethodAttributes.Public,
                    CallingConventions.Standard,
                    parameterTypes);

                ILGenerator il = constructorBuilder.GetILGenerator();
                // 调用父类的构造函数
                il.Emit(OpCodes.Ldarg_0);
                for (int j = 0; j < ctorParams.Length; j++) {
                    il.Emit(OpCodes.Ldarg, j + 3);
                }
                il.Emit(OpCodes.Call, ConstructorInfos[i]);
                // 赋值拦截器
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, FieldBuilders[InterceptorsFieldName]);
                // 赋值targetType
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Stfld, FieldBuilders[TargetTypeFieldName]);
                il.Emit(OpCodes.Ret);
            }
        }

        protected override LocalBuilder CreateProxyInvocation(ILGenerator ilGenerator, LocalBuilder conArgumentsArray, LocalBuilder typeArray,
            LocalBuilder methodInfoLocal, LocalBuilder targetMethodInfoLocal) {
            // 生成InheritInvocation对象
            LocalBuilder proxyInvocationLocal = ilGenerator.DeclareLocal(typeof(InheritInvocation));
            var ci = typeof(InheritInvocation).GetConstructor(
                new[]
                { typeof(object[]), typeof(object), typeof(Type),
                  typeof(IInterceptor[]), typeof(Type[]), typeof(MethodInfo), typeof(MethodInfo) });
            //构造函数参数
            ilGenerator.Emit(OpCodes.Ldloc, conArgumentsArray);
            //代理对象this
            ilGenerator.Emit(OpCodes.Ldarg_0);
            //targetType
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, FieldBuilders[TargetTypeFieldName]);
            //拦截器
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, FieldBuilders[InterceptorsFieldName]);
            //泛型参数类型数组
            ilGenerator.Emit(OpCodes.Ldloc, typeArray);
            //拦截方法
            ilGenerator.Emit(OpCodes.Ldloc, methodInfoLocal);
            //真实方法
            ilGenerator.Emit(OpCodes.Ldloc, targetMethodInfoLocal);
            ilGenerator.Emit(OpCodes.Newobj, ci);
            ilGenerator.Emit(OpCodes.Stloc, proxyInvocationLocal);
            return proxyInvocationLocal;
        }
    }
}