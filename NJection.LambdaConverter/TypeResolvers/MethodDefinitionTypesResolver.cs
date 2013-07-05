using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace NJection.LambdaConverter.TypeResolvers
{
    internal class MethodDefinitionTypesResolver : CecilArgumentsResolver
    {
        public MethodDefinitionTypesResolver(MethodDefinition methodDefinition, Type[] types)
            : base(MethodDictionaryResolver.Resolve(methodDefinition, types)) { }

        private static class MethodDictionaryResolver
        {
            public static IDictionary<string, TypeReferencePair> Resolve(MethodDefinition methodDefinition, Type[] types) {
                var methodParameters = methodDefinition.GenericParameters;
                var assemblyResolver = methodDefinition.Module.AssemblyResolver;

                return types.Select((type, i) => {
                    var parameter = methodParameters[i];
                    var moudle = type.Assembly.GetName().FullName;
                    var assembly = assemblyResolver.Resolve(moudle);
                    return new {
                        Key = parameter.FullName,
                        Type = type,
                        TypeRefence = assembly.MainModule.Import(type)
                    };
                })
                .ToDictionary(kv => kv.Key, kv => new TypeReferencePair(kv.Type, kv.TypeRefence));
            }
        }
    }
}