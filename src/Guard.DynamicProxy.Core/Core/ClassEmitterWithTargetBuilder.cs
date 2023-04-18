using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Guard.DynamicProxy.Abstracts.Interfaces;
using Guard.DynamicProxy.Core.Invocation;

namespace Guard.DynamicProxy.Core.Core {
    public class ClassEmitterWithTargetBuilder : AbstractEmitterBuilder {
        internal const string TargetFieldName = "_target";
        public object Target { get; }

        public ClassEmitterWithTargetBuilder(TypeBuilder typeBuilder, Type targetType, object target)
            : base(typeBuilder, targetType) {
            Target = target;
        }

        protected override void CreateConstructor() {
                Type[] parameterTypes = new Type[3];
                parameterTypes[0] = typeof(IInterceptor[]);
                parameterTypes[1] = typeof(Type);
                parameterTypes[2] = typeof(object);
                // 生成构造函数，添加额外的参数一个是拦截器，一个是targetType
                ConstructorBuilder constructorBuilder = TypeBuilder.DefineConstructor(
                    MethodAttributes.Public,
                    CallingConventions.Standard,
                    parameterTypes);

                ILGenerator il = constructorBuilder.GetILGenerator();
                // 调用父类的构造函数
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Callvirt, typeof(object).GetConstructor(new Type[0]));
                // 赋值拦截器
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, FieldBuilders[InterceptorsFieldName]);
                // 赋值targetType
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Stfld, FieldBuilders[TargetTypeFieldName]);
                // 赋值target
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_3);
                il.Emit(OpCodes.Stfld, FieldBuilders[TargetFieldName]);
                il.Emit(OpCodes.Ret);
        }
        protected override LocalBuilder CreateProxyInvocation(ILGenerator ilGenerator, LocalBuilder conArgumentsArray,
            LocalBuilder typeArray,
            LocalBuilder methodInfoLocal, LocalBuilder targetMethodInfoLocal) {
            LocalBuilder proxyInvocationLocal = ilGenerator.DeclareLocal(typeof(InheritWithTargetInvocation));
            var ci = typeof(InheritWithTargetInvocation).GetConstructor(
                new[]
                { typeof(object[]), typeof(object), typeof(object), typeof(Type),
                  typeof(IInterceptor[]), typeof(Type[]), typeof(MethodInfo), typeof(MethodInfo) });
            //构造函数参数
            ilGenerator.Emit(OpCodes.Ldloc, conArgumentsArray);
            //目标示例
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, FieldBuilders[TargetFieldName]);
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

