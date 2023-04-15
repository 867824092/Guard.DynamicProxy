using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Guard.DynamicProxy.Abstracts.Interfaces;

namespace Guard.DynamicProxy.Core.Core{

    public abstract class AbstractEmitterBuilder {
        public const string InterceptorsFieldName = "_interceptors";
        public const string TargetTypeFieldName = "_targetType";
        protected List<MethodInfo> MethodInfos { get; }
        protected List<ConstructorInfo> ConstructorInfos { get; }
        protected Dictionary<string, FieldBuilder> FieldBuilders { get; }
        protected TypeBuilder TypeBuilder { get; }
        protected Type TargetType { get; }

        protected AbstractEmitterBuilder(TypeBuilder typeBuilder, Type targetType) {
            TypeBuilder = typeBuilder;
            TargetType = targetType;
            ConstructorInfos = new List<ConstructorInfo>();
            FieldBuilders = new Dictionary<string, FieldBuilder>();
            MethodInfos = new List<MethodInfo>();
        }

        public AbstractEmitterBuilder AddPrivateFields(string name, Type type,
            FieldAttributes flags = FieldAttributes.Private) {
            var fieldBuilder = TypeBuilder.DefineField(name, type, flags);
            FieldBuilders.Add(name, fieldBuilder);
            return this;
        }

        public AbstractEmitterBuilder AddConstructors(bool isInherit = true) {
            //查找目标类型的所有公共构造函数，如果没有则报错
            if (TargetType.GetConstructors().Length == 0) {
                throw new Exception("目标类型没有公共构造函数");
            }

            if (isInherit) {
                ConstructorInfos.AddRange(TargetType.GetConstructors());
            }else {
                // 重新获取构造函数，不包含继承的构造函数
                ConstructorInfos.Add(typeof(object).GetConstructor(Type.EmptyTypes));
            }
            return this;
        }

        public AbstractEmitterBuilder AddMethods() {
            var methodInfos =
                TargetType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (methodInfos.Length == 0) return this;
            //TODO 需要新增对特殊方法的去除
            foreach (var methodInfo in methodInfos) {
                if (methodInfo.IsPublic && methodInfo.IsVirtual) {
                    MethodInfos.Add(methodInfo);
                }
            }

            return this;
        }

        protected abstract void CreateConstructor();
        protected void CreateMethos() {

            foreach (MethodInfo method in MethodInfos) {
                Type[] genericArguments = method.GetGenericArguments();
                string[] argumentNames = new string[genericArguments.Length];
                for (int i = 0; i < genericArguments.Length; i++) {
                    argumentNames[i] = genericArguments[i].Name;
                }

                ParameterInfo[] parameters = method.GetParameters();
                Type[] parameterTypes = new Type[parameters.Length];
                for (int i = 0; i < parameters.Length; i++) {
                    parameterTypes[i] = parameters[i].ParameterType;
                }

                //TODO 此处如果是调用了目标对象生成，则直接调用目标对象的方法，不需要新增方法
                //新增方法 xxxx_原始方法，用于调用父类的方法
                var targetMethod = TypeBuilder.DefineMethod(TypeBuilder.Name + "_" + method.Name,
                    MethodAttributes.Public | MethodAttributes.HideBySig,
                    method.ReturnType,
                    parameters.Select(p => p.ParameterType).ToArray());
                if (argumentNames.Length > 0) {
                    //声明泛型参数
                    targetMethod.DefineGenericParameters(argumentNames);
                }

                //调用父类方法
                ILGenerator il = targetMethod.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                for (int i = 1; i <= parameters.Length; i++) {
                    il.Emit(OpCodes.Ldarg, i);
                }

                il.Emit(OpCodes.Call, method);
                il.Emit(OpCodes.Ret);
                //重写父类方法
                MethodBuilder methodBuilder = TypeBuilder.DefineMethod(method.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual,
                    method.ReturnType,
                    parameterTypes);
                //定义泛型参数
                il = methodBuilder.GetILGenerator();
                //泛型参数类型数组
                LocalBuilder typeArray = il.DeclareLocal(typeof(Type[]));
                il.Emit(OpCodes.Ldc_I4, genericArguments.Length);
                il.Emit(OpCodes.Newarr, typeof(Type));
                il.Emit(OpCodes.Stloc, typeArray);
                if (genericArguments.Length > 0) {
                    GenericTypeParameterBuilder[] genericTypeParameterBuilders =
                        methodBuilder.DefineGenericParameters(argumentNames);
                    for (int i = 0; i < genericArguments.Length; i++) {
                        il.Emit(OpCodes.Ldloc, typeArray);
                        il.Emit(OpCodes.Ldc_I4, i);
                        il.Emit(OpCodes.Ldtoken, genericTypeParameterBuilders[i]);
                        il.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
                        il.Emit(OpCodes.Stelem_Ref);
                    }
                }

                //创建参数数组
                LocalBuilder objectArray = il.DeclareLocal(typeof(object[]));
                il.Emit(OpCodes.Ldc_I4, parameters.Length);
                il.Emit(OpCodes.Newarr, typeof(object));
                il.Emit(OpCodes.Stloc, objectArray);
                for (int i = 0; i < parameters.Length; i++) {
                    //读取数组对象
                    il.Emit(OpCodes.Ldloc, objectArray);
                    //索引
                    il.Emit(OpCodes.Ldc_I4, i);
                    //读取方法上的参数
                    il.Emit(OpCodes.Ldarg_S, i + 1);
                    //值对象装箱
                    if (parameterTypes[i].IsValueType || parameterTypes[i].IsGenericParameter) {
                        il.Emit(OpCodes.Box, parameterTypes[i]);
                    }
                    il.Emit(OpCodes.Stelem_Ref);
                }
                //方法对象
                LocalBuilder methodInfoLocal = il.DeclareLocal(typeof(MethodInfo));
                il.Emit(OpCodes.Ldtoken, method);
                il.Emit(OpCodes.Call, typeof(MethodBase).GetMethod("GetMethodFromHandle", new[]
                { typeof(RuntimeMethodHandle) }));
                il.Emit(OpCodes.Castclass, typeof(MethodInfo));
                il.Emit(OpCodes.Stloc, methodInfoLocal);
                //真实方法Method对象
                LocalBuilder targetMethodInfoLocal = il.DeclareLocal(typeof(MethodInfo));
                il.Emit(OpCodes.Ldtoken, targetMethod);
                il.Emit(OpCodes.Call, typeof(MethodBase).GetMethod("GetMethodFromHandle", new[]
                { typeof(RuntimeMethodHandle) }));
                il.Emit(OpCodes.Castclass, typeof(MethodInfo));
                il.Emit(OpCodes.Stloc, targetMethodInfoLocal);
                // 生成Invocation对象
                LocalBuilder proxyInvocationLocal = CreateProxyInvocation(il, objectArray, typeArray,
                    methodInfoLocal, targetMethodInfoLocal);
                //执行invocation对象的Proceed方法
                il.Emit(OpCodes.Ldloc, proxyInvocationLocal);
                il.Emit(OpCodes.Callvirt, typeof(IInvocation).GetMethod("Proceed"));
                //返回值
                if (method.ReturnType != typeof(void)) {
                    il.Emit(OpCodes.Ldloc, proxyInvocationLocal);
                    il.Emit(OpCodes.Callvirt, typeof(IInvocation).GetMethod("get_ReturnValue"));
                    if (method.ReturnType.IsValueType) {
                        il.Emit(OpCodes.Unbox_Any, method.ReturnType);
                    }
                }
                il.Emit(OpCodes.Ret);
            }
        }

        protected abstract LocalBuilder CreateProxyInvocation(ILGenerator ilGenerator,
            LocalBuilder conArgumentsArray,
            LocalBuilder typeArray,
            LocalBuilder methodInfoLocal,
            LocalBuilder targetMethodInfoLocal);

        public Type Build() {
            CreateConstructor();
            CreateMethos();
            return TypeBuilder.CreateTypeInfo();
        }
    }
}