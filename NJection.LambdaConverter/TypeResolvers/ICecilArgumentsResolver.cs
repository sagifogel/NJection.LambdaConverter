using System;
using Mono.Cecil;

namespace NJection.LambdaConverter.TypeResolvers
{
    internal interface ICecilArgumentsResolver
    {
        bool Contains(string argument);
        Type ResolveType(string argument);
        TypeReferencePair ResolvePair(string argument);
        TypeReference ResolveTypeReference(string argument);
    }
}