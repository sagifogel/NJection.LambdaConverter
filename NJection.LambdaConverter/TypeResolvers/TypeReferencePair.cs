using System;
using Mono.Cecil;

namespace NJection.LambdaConverter.TypeResolvers
{
    internal class TypeReferencePair
    {
        internal TypeReferencePair(Type type, TypeReference typeReference) {
            Type = type;
            TypeReference = typeReference;
        }

        public Type Type { get; private set; }
        public TypeReference TypeReference { get; private set; }
    }
}