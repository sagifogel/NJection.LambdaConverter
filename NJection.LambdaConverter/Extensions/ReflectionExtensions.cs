using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace NJection.LambdaConverter.Extensions
{
    internal static class ReflectionUtils
    {
        private static readonly Type _delegate = typeof(Delegate);

        private delegate Type ResolveDelegateType(params Type[] typeArgs);

        public static readonly BindingFlags AllFlags = BindingFlags.Public |
                                                       BindingFlags.NonPublic |
                                                       BindingFlags.Instance |
                                                       BindingFlags.Static |
                                                       BindingFlags.FlattenHierarchy;

        internal static bool IsDelegate(this Type type) {
            return _delegate.IsAssignableFrom(type);
        }

        internal static bool IsStatic(this Type type) {
            return type.IsAbstract && type.IsSealed;
        }

        internal static bool IsExtensionMethod(this MethodInfo method) {
            return method.IsDefined(typeof(ExtensionAttribute), true);
        }

        internal static Delegate CreateDelegate(this MethodInfo methodInfo, Type type = null, object @this = null) {
            if (type == null) {
                Type[] types = null;
                ResolveDelegateType getDelegateType = null;
                ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                bool methodHasReturnType = !methodInfo.ReturnType.Equals(TypeSystem.Void);
                var parameterTypes = parameterInfos.Select(parameter => parameter.ParameterType);

                if (methodHasReturnType) {
                    parameterTypes = parameterTypes.Concat(methodInfo.ReturnType);
                    getDelegateType = Expression.GetFuncType;
                }
                else {
                    getDelegateType = Expression.GetActionType;
                }

                types = parameterTypes.ToArray();
                type = getDelegateType(types);
            }

            return Delegate.CreateDelegate(type, @this, methodInfo);
        }

        internal static Type MakeGenericType(this Type type, TypeBuilder typeBuilder) {
            GenericTypeParameterBuilder[] genericTypeParameterBuilder = null;
            Type[] genericArguments = type.GetGenericArguments();
            string[] genericParameters = genericArguments.Select(g => g.Name)
                                                         .ToArray();

            genericTypeParameterBuilder = typeBuilder.DefineGenericParameters(genericParameters);

            return typeBuilder.MakeGenericType(genericTypeParameterBuilder);
        }

        internal static Type GetNonNullableType(this Type type) {
            if (type.IsNullableType()) {
                return type.GetGenericArguments()[0];
            }

            return type;
        }

        internal static bool IsInt32OrInt64(this Type type) {
            return type == TypeSystem.Int || type == TypeSystem.Long;
        }

        internal static bool IsSingleOrDouble(this Type type) {
            return type == TypeSystem.Float || type == TypeSystem.Double;
        }

        internal static bool IsFloatingPoint(this Type type) {
            type = type.GetNonNullableType();

            switch (Type.GetTypeCode(type)) {
                case TypeCode.Single:
                case TypeCode.Double:

                    return true;

                default:

                    return false;
            }
        }

        internal static bool IsUnsigned(this Type type) {
            type = type.GetNonNullableType();

            switch (Type.GetTypeCode(type)) {
                case TypeCode.Char:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
            }
            return false;
        }

        internal static bool IsNullableType(this Type type) {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsEquivalentTo(this Type type1, Type type2) {
            if (!(type1 == type2)) {
                return type1.IsEquivalentTo(type2);
            }

            return true;
        }

        internal static bool ContainsGenericParameters(this Type type, out Type[] arguments) {
            arguments = Type.EmptyTypes;

            if (type.IsGenericType) {
                if (type.ContainsGenericParameters) {
                    arguments = type.GetGenericArguments();
                }

                return true;
            }

            return false;
        }

        internal static bool IsCompilerGenerated(this Type type) {
            return type.IsDefined(typeof(CompilerGeneratedAttribute), true);
        }

        internal static bool IsSubType(this Type type, Type subType) {
            return subType.IsSubclassOf(type) || type.GetInterfaces().Any(i => i.Equals(subType));
        }

        internal static MemberInfo[] GetMember(this Type type, string name, BindingFlags bindingAttr, MemberTypes memberTypes = MemberTypes.All, bool includeSubTypes = false) {
            MemberInfo[] memberInfos = type.GetMember(name, memberTypes, bindingAttr);

            if (memberInfos.Length == 0 && includeSubTypes) {
                memberInfos = type.GetInterfaces()
                                  .Select(t => t.GetMember(name, bindingAttr))
                                  .FirstOrDefault(t => t.Length > 0);
            }

            return memberInfos;
        }

        internal static Type GetMemberType(this MemberInfo member) {
            Type type = null;

            switch (member.MemberType) {
                case MemberTypes.Event:

                    type = (member as EventInfo).EventHandlerType;
                    break;

                case MemberTypes.Field:

                    type = (member as FieldInfo).FieldType;
                    break;

                case MemberTypes.Method:

                    type = (member as MethodInfo).ReturnType;
                    break;

                case MemberTypes.Property:

                    type = (member as PropertyInfo).PropertyType;
                    break;

                default:

                    throw new NotSupportedException();
            }

            return type;
        }

        internal static Type MakeSafeArrayType(this Type type, Nullable<int> rank = 0) {
            return rank > 1 ? type.MakeArrayType(rank.Value) : type.MakeArrayType();
        }

        internal static bool IsPrimitive(this Type type) {
            return type.IsPrimitive || type.IsValueType || Type.GetTypeCode(type) == TypeCode.Decimal;
        }

        internal static bool IsNumeric(this Type type) {
            type = type.GetNonNullableType();

            if (type.IsEnum) {
                return true;
            }

            switch (Type.GetTypeCode(type)) {
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                    return true;
            }

            return false;
        }

        public static bool IsGenericListOrDictionary(this Type type) {
            if (type.IsGenericType) {
                var genericDefinition = type.GetGenericTypeDefinition();

                return genericDefinition == typeof(List<>) ||
                       genericDefinition == typeof(Dictionary<,>);
            }

            return false;
        }
    }
}