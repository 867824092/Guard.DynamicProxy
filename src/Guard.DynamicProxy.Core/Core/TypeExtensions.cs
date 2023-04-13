using System;

namespace Guard.DynamicProxy.Core.Core {
    /// <summary>
    /// 类型操作帮助类
    /// </summary>
    public static class TypeExtensions {
        /// <summary>
        /// 检测类型是否为null
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="argumentName">参数名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void CheckNotNullType(this Type type,string argumentName) {
            if (type == null) {
                throw new ArgumentNullException(argumentName);
            }
        }
        /// <summary>
        /// 检测类型是否为开放泛型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="argumentName">参数名称</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CheckNotGenericType(this Type type,string argumentName) {
            type.CheckNotNullType(argumentName);
            if (type != null && type.IsGenericTypeDefinition)
            {
                throw new ArgumentException($"不能创建开放泛型 {type.FullName??type.Name}", argumentName);
            }
        }
        /// <summary>
        /// 检测类型是否为class类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="argumentName">参数名称</param>
        /// <exception cref="ArgumentException"></exception>
        public static void CheckIsClass(this Type type,string argumentName) {
            type.CheckNotNullType(argumentName);
            if (!type.IsClass)
            {
                throw new ArgumentException("targetType必须是class类型", argumentName);
            }
        }
    }
}