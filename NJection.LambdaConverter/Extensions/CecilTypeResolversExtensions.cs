using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace NJection.LambdaConverter.Extensions
{
    internal static class CecilTypeResolversExtensions
    {
        private static readonly BindingFlags _flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic;

        internal static TMethod GetActualMethod<TMethod>(this MethodReference methodReference) where TMethod : MethodBase {
            return GetActualMethod(methodReference) as TMethod;
        }

        internal static MethodBase GetActualMethod(this MethodReference methodReference) {
            MethodInfo methodInfo = null;
            Type[] types = Type.EmptyTypes;
            Type type = methodReference.DeclaringType.GetActualType();

            if (methodReference.Name.Equals(".ctor")) {
                return methodReference.GetActualConstructor(type);
            }

            if (methodReference.IsGeneric()) {
                return methodReference.GetGenericActualMethod();
            }

            if (!type.ContainsGenericParameters(out types)) {
                types = methodReference.Parameters
                                       .Select(p => p.ParameterType.GetActualType())
                                       .ToArray();
            }

            MethodInfo[] methodInfos = type.GetMember(methodReference.Name, MemberTypes.Method, _flags)
                                           .Cast<MethodInfo>()
                                           .Where(m => !m.IsGenericMethodDefinition)
                                           .ToArray();

            if (methodInfos.Length > 1) {
                methodInfo = Type.DefaultBinder.SelectMethod(_flags, methodInfos, types, null) as MethodInfo;
            }
            else {
                methodInfo = methodInfos[0];
            }

            return methodInfo;
        }

        internal static FieldInfo GetActualField(this FieldReference fieldReference) {
            Type type = fieldReference.DeclaringType.GetActualType();

            return GetActualMember<FieldInfo>(fieldReference, MemberTypes.Field);
        }

        internal static EventInfo GetActualEvent(this EventReference eventReference) {
            return GetActualMember<EventInfo>(eventReference, MemberTypes.Event);
        }

        internal static PropertyInfo GetActualProperty(this PropertyReference propertyReference) {
            return GetActualMember<PropertyInfo>(propertyReference, MemberTypes.Property);
        }

        private static TMemberInfo GetActualMember<TMemberInfo>(this MemberReference memberReference, MemberTypes memberType) where TMemberInfo : MemberInfo {
            Type declaringType = memberReference.DeclaringType.GetActualType();

            return declaringType.GetMember(memberReference.Name, _flags, memberType)[0] as TMemberInfo;
        }

        private static ConstructorInfo GetActualConstructor(this MethodReference methodReference, Type declaringType) {
            ConstructorInfo constructorInfo = null;
            var constructorInfos = declaringType.GetMember(methodReference.Name, MemberTypes.Constructor, _flags)
                                                .ToArrayOf<ConstructorInfo>();

            if (constructorInfos.Length > 1) {
                Type[] types = Type.EmptyTypes;

                if (methodReference.HasParameters) {
                    if (methodReference.IsGeneric()) {
                        types = declaringType.GetGenericArguments();
                    }
                    else {
                        types = methodReference.Parameters
                                          .Select(p => p.ParameterType.GetActualType())
                                          .ToArray();
                    }
                }

                constructorInfo = Type.DefaultBinder.SelectMethod(_flags, constructorInfos, types, null) as ConstructorInfo;
            }
            else {
                constructorInfo = constructorInfos[0];
            }

            return constructorInfo;
        }

        private static MethodInfo GetGenericActualMethod(this MethodReference methodReference, params TypeReference[] typeReferences) {
            Type[] types = null;
            MethodInfo methodInfo = null;
            MethodInfo[] methodInfos = null;

            types = methodReference.GenericParameters
                                   .Select(p => p.GetActualType())
                                   .ToArray();

            methodInfos = methodReference.DeclaringType
                                         .GetActualType()
                                         .GetMember(methodReference.Name, _flags)
                                         .ToArrayOf<MethodInfo>();

            if (methodInfos.Length > 1) {
                methodInfo = methodInfos.FirstOrDefault(method => MethodMatch(methodReference, method));
            }
            else {
                methodInfo = methodInfos[0];
            }

            if (methodInfo.IsGenericMethod) {
                methodInfo = methodInfo.MakeGenericMethod(types);
            }

            return methodInfo;
        }

        internal static Type GetActualType(this ArrayType arrayType) {
            Type elementType = arrayType.ElementType.GetActualType();

            if (arrayType.Rank == 1) {
                return elementType.MakeArrayType();
            }

            return elementType.MakeArrayType(arrayType.Rank);
        }

        internal static Type GetActualType(this TypeReference typeReference) {
            Assembly assembly = null;
            ArrayType arrayType = null;
            string assemblyName = string.Empty;

            assemblyName = typeReference.Scope is ModuleDefinition ?
                                (typeReference.Scope as ModuleDefinition).Assembly.Name.FullName :
                                (typeReference.Scope as AssemblyNameReference).FullName;

            if (!AssemblyResolver.Instance.TryResolve(assemblyName, out assembly)) {
                throw new ArgumentException();
            }

            if (typeReference.IsGenericInstance) {
                var genericInstanceType = typeReference as GenericInstanceType;

                return genericInstanceType.GetGenericActualType(assembly);
            }

            if (typeReference.IsGenericParameter) {
                var genericParameter = typeReference as GenericParameter;
                arrayType = genericParameter.Owner as ArrayType;

                if (arrayType == null) {
                    return genericParameter.GetGenericParameterActualType(assembly);
                }
            }
            else {
                arrayType = typeReference as ArrayType;
            }

            if (arrayType != null) {
                return arrayType.GetActualType();
            }

            return assembly.GetType(typeReference.FullName.Replace("/", "+"));
        }

        internal static bool NeedsTypeResolving(this TypeReference typeReference) {
            if (typeReference.IsGenericInstance) {
                var genericInstance = typeReference as IGenericInstance;

                return genericInstance.GenericArguments
                                      .Any(g => g.NeedsTypeResolving());
            }

            return typeReference.IsGenericParameter;
        }

        internal static bool IsGeneric(this TypeReference typeReference) {
            var genericType = typeReference.GetGenericTypeOrSelfReference();

            return IsGenericParameterOrInstance(genericType);
        }

        internal static bool IsGenericParameterOrInstance(this TypeReference genericType) {
            return genericType.IsGenericParameter || genericType.IsGenericInstance;
        }

        internal static TypeReference GetGenericActualType(this TypeReference typeReference, IEnumerable<TypeReference> referenceTypes) {
            var genericInstance = typeReference as GenericInstanceType;

            if (genericInstance != null && genericInstance.HasGenericArguments) {
                var genericArguments = genericInstance.GenericArguments.ToList();
                genericInstance.GenericArguments.Clear();

                genericArguments.Select(g => g.GetGenericActualType(referenceTypes))
                                .ForEach(g => genericInstance.GenericArguments.Add(g));

                return genericInstance;
            }

            if (!typeReference.IsGeneric()) {
                return typeReference;
            }

            return typeReference.GetGenericTypeReference(referenceTypes.ToArray());
        }

        internal static TypeReference GetGenericTypeReference(this TypeReference typeReference, TypeReference[] referenceTypes) {
            Regex regex = new Regex(@"!(?<T>\d{1})");
            MatchCollection matches = regex.Matches(typeReference.FullName);

            return matches.Cast<Match>()
                          .Select(m => {
                              int index;

                              if (!int.TryParse(m.Groups["T"].Value, out index)) {
                                  throw new ArgumentException("Could not resolve type reference");
                              }

                              return referenceTypes[index];
                          })
                          .FirstOrDefault();
        }

        private static Type GetGenericActualType(this GenericInstanceType typeReference, Assembly assembly) {
            StringBuilder stringBuilder = new StringBuilder();
            Type[] types = new Type[typeReference.GenericArguments.Count];
            string typeFullName = GetFullName(typeReference.FullName.Replace("/", "+"));

            stringBuilder.AppendFormat("{0}[", typeFullName.Split('[')[0]);

            typeReference.GenericArguments.ForEach((p, i) => {
                Type type = p.GetActualType();

                if (type != null) {
                    stringBuilder.AppendFormat("[{0}],", type.AssemblyQualifiedName);
                }
            });

            stringBuilder.Remove(stringBuilder.Length - 1, 1).Append("]");

            return assembly.GetType(stringBuilder.ToString());
        }

        private static Type GetGenericParameterActualType(this GenericParameter typeReference, Assembly assembly) {
            StringBuilder stringBuilder = new StringBuilder();
            string typeFullName = GetFullName(typeReference.FullName.Replace("/", "+"));
            var genericParameters = typeReference.GenericParameters;

            stringBuilder.AppendFormat("{0}[", typeFullName.Split('[')[0]);

            if (typeReference.HasGenericParameters) {
                genericParameters.ForEach((p, i) => {
                    Type type = genericParameters[i].GetActualType();
                    string name = type.AssemblyQualifiedName;
                    stringBuilder.AppendFormat("[{0}],", name);
                });

                stringBuilder.Remove(stringBuilder.Length - 1, 1).Append("]");
                typeFullName = stringBuilder.ToString();
            }

            return assembly.GetType(typeFullName);
        }

        internal static TypeReference GetGenericTypeOrSelfReference(this TypeReference typeReference) {
            TypeReference element = typeReference;

            if (typeReference.IsGenericParameter || typeReference.IsGenericInstance) {
                return typeReference;
            }

            for (var self = element.GetElementType(); self != element; ) {
                element = self;
            }

            if (typeReference.IsByReference && !element.IsGenericParameterOrInstance()) {
                return typeReference;
            }

            return element;
        }

        internal static bool MethodMatch(this MethodReference method, MethodInfo info) {
            if (!method.Name.Equals(info.Name) || !MatchReturnType(method.ReturnType, info.ReturnType)) {
                return false;
            }

            return MatchParameters(method.Parameters, info.GetParameters());
        }

        internal static bool CtorMatch(this MethodReference method, ConstructorInfo info) {
            return method.Name.Equals(info.Name) &&
                   MatchParameters(method.Parameters, info.GetParameters());
        }

        private static bool MatchReturnType(TypeReference returnTypeReference, Type returnType) {
            return returnTypeReference.Name.Equals(returnType.Name) || IsGeneric(returnTypeReference, returnType);
        }

        private static bool MatchParameters(Collection<ParameterDefinition> parameters, ParameterInfo[] infos) {
            if (parameters.Count != infos.Length) {
                return false;
            }

            return parameters.All((p, i) => {
                return MatchParameter(p, infos[i]);
            });
        }

        private static bool MatchParameter(ParameterDefinition parameter, ParameterInfo info) {
            TypeReference typeReference = parameter.ParameterType;
            Type parameterType = typeReference.GetActualType();

            return info.ParameterType.Equals(parameterType) || IsGeneric(typeReference, info.ParameterType);
        }

        private static bool HasSameElementType(TypeReference parameter, Type paremeterType) {
            if (paremeterType.HasElementType) {
                parameter = parameter.GetElementType();
                return paremeterType.GetElementType().Equals(parameter);
            }

            return false;
        }

        private static bool IsGeneric(TypeReference typeReference, Type type) {
            bool typesCanNotBeCompared = typeReference.IsGenericParameter || type.IsGenericParameter;

            if (!typesCanNotBeCompared) {
                typeReference = typeReference.GetGenericTypeOrSelfReference();

                if (typeReference.IsGenericInstance) {
                    var genericType = typeReference as GenericInstanceType;
                    var typeArguments = type.GetGenericArguments();
                    var genericArgumentsCount = genericType.GenericArguments.Count;
                    var elementType = typeReference.GetElementType();
                    var actualType = elementType.GetActualType();

                    if (elementType.Name.Equals(actualType.Name) && genericArgumentsCount == typeArguments.Length) {
                        return genericType.GenericArguments.All((p, i) => {
                            return IsGeneric(p, typeArguments[i]);
                        });
                    }

                    return false;
                }

                if (type.HasElementType) {
                    Type elementType = null;

                    if (!type.IsByRef) {
                        return IsGeneric(typeReference, type.GetElementType());
                    }

                    do {
                        elementType = type.GetElementType();
                    }
                    while (elementType.HasElementType);

                    if (elementType.IsGeneric() || typeReference.IsGeneric()) {
                        return true;
                    }
                }
            }

            return typesCanNotBeCompared || type.Equals(typeReference.GetActualType());
        }

        private static bool IsGeneric(this Type type) {
            return type.IsGenericParameter || type.IsGenericType || type.IsGenericTypeDefinition;
        }

        private static string GetFullName(string fullName) {
            int startIndex = 0;
            int fullNameLength = fullName.Length;

            if (fullName.Length > 2) {
                startIndex = 2;
                fullNameLength = fullName.Length - 2;
            }

            return new StringBuilder(fullName).Replace("<", "[", startIndex, fullNameLength)
                                              .Replace(">", "]", startIndex, fullNameLength)
                                              .ToString();
        }

        internal static MethodDefinition MakeGenericMethod(this MethodDefinition methodDefinition, Type[] types) {
            IEnumerable<TypeReference> arguments = null;
            TypeReference returnType = methodDefinition.ReturnType;
            TypeDefinition typeReference = methodDefinition.DeclaringType.Resolve();
            var genericParameters = methodDefinition.GenericParameters;

            arguments = genericParameters.Join(methodDefinition.Parameters
                                                               .Where(p => p.ParameterType.IsGeneric())
                                                               .Select(p => p.ParameterType),
                                               gp => gp.Name,
                                               p => p.GetElementType().Name,
                                               (gp, p) => {
                                                   Type type = null;
                                                   int index = genericParameters.IndexOf(gp);

                                                   type = types[index];

                                                   if (p.IsByReference) {
                                                       type = type.MakeByRefType();
                                                   }

                                                   var newType = new TypeReference(type.Namespace, type.Name, typeReference.Module, typeReference.Scope);
                                                   return typeReference.Module.Import(newType);
                                               });

            if (returnType.IsGeneric()) {
                if (returnType.IsGenericInstance) {
                    var genericRetuenType = returnType as IGenericInstance;

                    var referenceTypes = genericParameters.Join(genericRetuenType.GenericArguments,
                                                                p => p.FullName,
                                                                gp => gp.FullName,
                                                                (p, gp) => types[p.Position]);

                    var references = types.Select(t => typeReference.Module.Import(t));
                    var genericArguments = genericRetuenType.GenericArguments;

                    genericArguments.Clear();
                    references.ForEach(r => genericArguments.Add(r));
                }
                else {
                    int index = genericParameters.IndexOf(p => p.Name.Equals(returnType.Name));
                    returnType = returnType.Module.Import(types[index]);
                }

                arguments = arguments.Concat(returnType);
            }

            return methodDefinition.MakeGenericMethod(arguments.Distinct(a => a.FullName), returnType);
        }

        internal static MethodReference MakeGenericMethod(this MethodReference methodDefinition, IEnumerable<TypeReference> arguments, TypeReference returnType = null) {
            returnType = returnType ?? methodDefinition.ReturnType;

            var reference = new MethodReference(methodDefinition.Name, returnType) {
                DeclaringType = methodDefinition.DeclaringType,
                HasThis = methodDefinition.HasThis,
                ExplicitThis = methodDefinition.ExplicitThis,
                CallingConvention = methodDefinition.CallingConvention,
            };

            ResolveArguments(reference, methodDefinition.Parameters, arguments);

            return reference;
        }

        private static MethodDefinition MakeGenericMethod(this MethodDefinition methodDefinition, IEnumerable<TypeReference> arguments, TypeReference returnType = null) {
            returnType = returnType ?? methodDefinition.ReturnType;

            var definition = new MethodDefinition(methodDefinition.Name, methodDefinition.Attributes, returnType) {
                DeclaringType = methodDefinition.DeclaringType,
                HasThis = methodDefinition.HasThis,
                ExplicitThis = methodDefinition.ExplicitThis,
                CallingConvention = methodDefinition.CallingConvention,
                Body = methodDefinition.Body
            };

            ResolveArguments(definition, methodDefinition.Parameters, arguments);

            return definition;
        }

        private static TypeReference MakeGenericType(this TypeReference typeReference, IEnumerable<TypeReference> arguments) {
            if (typeReference.IsGenericParameter) {
                var genericInstanceType = typeReference as GenericParameter;
                return genericInstanceType.GetGenericActualType(arguments);
            }

            return null;
        }

        private static void ResolveArguments(this MethodReference methodDefinition, Collection<ParameterDefinition> parameters, IEnumerable<TypeReference> arguments) {
            parameters.ForEach(p => methodDefinition.Parameters.Add(p));

            arguments.ForEach(argument => {
                var elementType = argument;

                if (argument.IsByReference) {
                    elementType = argument.GetElementType();
                }

                var genericPraramter = ResolveGenericParameter(elementType);

                methodDefinition.GenericParameters.Add(genericPraramter);
            });
        }

        private static GenericParameter ResolveGenericParameter(TypeReference typeReference) {
            var genericPraramter = new GenericParameter(typeReference.FullName, typeReference);

            if (typeReference.IsGenericInstance) {
                var genericInstance = typeReference as GenericInstanceType;

                genericInstance.GenericArguments
                               .ForEach(a => {
                                   var parameter = ResolveGenericParameter(a);

                                   genericPraramter.GenericParameters.Add(parameter);
                               });
            }

            return genericPraramter;
        }

        private static bool IsGeneric(this MethodReference methodReference) {
            return methodReference.HasGenericParameters ||
                   methodReference.IsGenericInstance ||
                   methodReference.ReturnType.IsGenericInstance ||
                   methodReference.ReturnType.IsGenericParameter ||
                   methodReference.Parameters.Any(p => p.ParameterType.GetGenericTypeOrSelfReference().IsGenericParameter);
        }
    }
}