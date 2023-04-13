using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Guard.DynamicProxy.Abstracts.Interfaces;
using Guard.DynamicProxy.Core.Interal;

namespace Guard.DynamicProxy.Core.Core {
    public class ClassEmitterBuilder {
        public const string InterceptorsFieldName = "_interceptors";
        public const string TargetTypeFieldName = "_targetType";
        private readonly List<MethodInfo> _methodInfos;
        private readonly List<ConstructorInfo> _constructorInfos;
        private readonly Dictionary<string,FieldBuilder> _fieldBuilders;
        public TypeBuilder TypeBuilder { get; }
        public Type TargetType { get; }
        public ClassEmitterBuilder(TypeBuilder typeBuilder,Type targetType) {
            TypeBuilder = typeBuilder;
            TargetType = targetType;
            _constructorInfos = new List<ConstructorInfo>();
            _fieldBuilders = new Dictionary<string,FieldBuilder>();
            _methodInfos = new List<MethodInfo>();
        }
        
        public ClassEmitterBuilder AddPrivateFields(string name, Type type,FieldAttributes flags = FieldAttributes.Private) {
            var fieldBuilder = TypeBuilder.DefineField(name, type, flags);
            _fieldBuilders.Add(name, fieldBuilder);
            return this;
        }
        public ClassEmitterBuilder AddConstructors() {
            //查找目标类型的所有公共构造函数，如果没有则报错
            if (TargetType.GetConstructors().Length == 0) {
                throw new Exception("目标类型没有公共构造函数");
            }
            _constructorInfos.AddRange(TargetType.GetConstructors());
            return this;
        }
        public ClassEmitterBuilder AddMethods() {
            var methodInfos = TargetType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            if (methodInfos.Length == 0) return this;
            foreach (var methodInfo in methodInfos) {
                if (methodInfo.IsPublic && methodInfo.IsVirtual) {
                    _methodInfos.Add(methodInfo);
                }
            }
            return this;
        }
        
        private void CreateConstructor() {
            for (int i = 0; i < _constructorInfos.Count; i++) {
                ParameterInfo[] ctorParams = _constructorInfos[i].GetParameters();
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
                il.Emit(OpCodes.Call, _constructorInfos[i]);
                // 赋值拦截器
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, _fieldBuilders[InterceptorsFieldName]);
                // 赋值targetType
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Stfld, _fieldBuilders[TargetTypeFieldName]);
                il.Emit(OpCodes.Ret);
            }
        }

        private void CreateMethos() {
            foreach (MethodInfo method in _methodInfos) {
                // 公共的虚方法
                if (method.IsVirtual && method.IsPublic) {
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
                    //新增方法 xxxx_原始方法，用于调用父类的方法
                    var targetMethod = TypeBuilder.DefineMethod(TypeBuilder.Name + "_" + method.Name,
                        MethodAttributes.Public | MethodAttributes.HideBySig,
                        method.ReturnType,
                        parameters.Select(p => p.ParameterType).ToArray());
                    if (argumentNames.Length > 0) {
                        //声明泛型参数
                        targetMethod.DefineGenericParameters(argumentNames);
                    }
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
                    // //生成ProxyInvocation对象
                    LocalBuilder proxyInvocationLocal = il.DeclareLocal(typeof(CompositionInvocation));
                    var ci = typeof(CompositionInvocation).GetConstructor(
                        new[]
                        { typeof(object[]), typeof(object), typeof(Type),
                          typeof(IInterceptor[]), typeof(Type[]), typeof(MethodInfo), typeof(MethodInfo) });
                    //构造函数参数
                    il.Emit(OpCodes.Ldloc, objectArray);
                    //代理对象this
                    il.Emit(OpCodes.Ldarg_0);
                    //targetType
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, _fieldBuilders[TargetTypeFieldName]);
                    //拦截器
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, _fieldBuilders[InterceptorsFieldName]);
                    //泛型参数类型数组
                    il.Emit(OpCodes.Ldloc, typeArray);
                    //拦截方法
                    il.Emit(OpCodes.Ldloc, methodInfoLocal);
                    //真实方法
                    il.Emit(OpCodes.Ldloc, targetMethodInfoLocal);
                    il.Emit(OpCodes.Newobj, ci);
                    il.Emit(OpCodes.Stloc, proxyInvocationLocal);
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
        }
        public Type Build() {
            CreateConstructor();
            CreateMethos();
            return TypeBuilder.CreateTypeInfo();
        }
    }
}