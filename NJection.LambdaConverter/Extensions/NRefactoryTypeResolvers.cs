using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using NJection.LambdaConverter.Mappers;

namespace NJection.LambdaConverter.Extensions
{
    internal static class NRefactoryTypeResolvers
    {
        static NRefactoryTypeResolvers() {
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().First(a => Path.GetFileNameWithoutExtension(a.CodeBase).Equals("mscorlib"));
            AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(assembly.Location);
        }

        internal static Type GetActualType(this ParameterDeclaration paremeterDefinition) {
            var paremeterReference = paremeterDefinition.Annotation<ParameterReference>();

            return paremeterReference.ParameterType.GetActualType();
        }

        internal static Type GetActualType(this AstType astType) {
            ComposedType composedType = astType as ComposedType;

            if (composedType != null) {
                return composedType.GetActualType();
            }

            PrimitiveType primitiveType = astType as PrimitiveType;

            if (primitiveType != null) {
                return primitiveType.GetActualType();
            }

            MemberType memberType = astType as MemberType;

            if (memberType != null) {
                return memberType.GetActualType();
            }

            var simpleType = astType as SimpleType;

            if (simpleType != null) {
                return simpleType.GetActualType();
            }

            if (astType.HasAnnotations()) {
                var typeReference = astType.Annotation<TypeReference>();

                if (typeReference != null) {
                    return typeReference.GetActualType();
                }

                var typeDefinition = astType.Annotation<TypeDefinition>();

                if (typeDefinition != null) {
                    return typeDefinition.GetActualType();
                }
            }

            return null;
        }

        internal static Type GetActualType(this SimpleType simpleType) {
            var typeReference = simpleType.Annotations.FirstOrDefault() as Mono.Cecil.TypeReference;

            if (typeReference != null) {
                bool hasTypes = false;
                Type[] types = new Type[simpleType.TypeArguments.Count];

                simpleType.TypeArguments
                          .ForEach((a, i) => {
                              var type = a.GetActualType();

                              if (type != null) {
                                  types[i] = type;
                                  hasTypes = true;
                              }
                          });

                if (hasTypes && typeReference.HasGenericParameters) {
                    return typeReference.GetActualType()
                                        .MakeGenericType(types);
                }

                return typeReference.GetActualType();
            }

            return null;
        }

        internal static Type GetActualType(this PrimitiveType primitiveType) {
            return TypeSystemMapper.Map(primitiveType.Keyword);
        }

        internal static Type GetActualType(this ComposedType composedType) {
            Type baseType = GetActualType(composedType.BaseType);

            composedType.ArraySpecifiers
                        .Reverse()
                        .ForEach(s => {
                            int dimensions = s.Dimensions;
                            baseType = dimensions > 1 ? baseType.MakeArrayType(dimensions) : baseType.MakeArrayType();
                        });

            return baseType;
        }

        internal static Type GetArrayType(this ArrayCreateExpression arrayCreateExpression) {
            var composedType = arrayCreateExpression.Type as ComposedType;
            var type = composedType.GetActualType();
            var arguments = arrayCreateExpression.Arguments;
            var additionalSpecifiers = arrayCreateExpression.AdditionalArraySpecifiers;
            var specifiers = additionalSpecifiers.Reverse()
                                                 .Select(a => a.Dimensions);

            if (arguments.Count > 0) {
                specifiers = specifiers.Concat(arguments.Count);
            }

            specifiers.ForEach(rank => {
                type = type.MakeSafeArrayType(rank);
            });

            return type;
        }

        internal static Type GetActualType(this MemberType memberType) {
            Mono.Cecil.TypeReference typeReference = null;
            object type = memberType.Annotations.First();
            TypeDefinition typeDefinition = type as TypeDefinition;

            if (typeDefinition != null) {
                return typeDefinition.GetActualType();
            }

            typeReference = type as Mono.Cecil.TypeReference;

            if (typeReference != null) {
                if (typeReference.HasGenericParameters) {
                    Type[] parameters = new Type[typeReference.GenericParameters.Count];
                    Type genericType = typeReference.GetActualType();
                    var simpleType = memberType.Target as SimpleType;

                    simpleType.TypeArguments
                              .ForEach((p, i) => {
                                  Type parameterType = p.GetActualType();
                                  parameters[i] = parameterType;
                              });

                    return genericType.MakeGenericType(parameters);
                }

                return typeReference.GetActualType();
            }

            return null;
        }

        internal static bool HasAnnotationOf<T>(this AstNode node, out T annotation) where T : class {
            annotation = node.Annotation<T>();

            return annotation != null;
        }

        internal static bool HasAnnotationOf<T>(this AstNode node) where T : class {
            T annotation;

            return node.HasAnnotationOf<T>(out annotation);
        }

        internal static bool HasAnnotations(this AstNode node) {
            return node.Annotations.Count() > 0;
        }
    }
}