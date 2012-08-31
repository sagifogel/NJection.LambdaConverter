using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Mono.Cecil;

namespace NJection.LambdaConverter.TypeResolvers
{
    internal class CecilArgumentsResolver : ICecilArgumentsResolver
    {
        private ConcurrentDictionary<string, TypeReferencePair> _dictionary = null;

        internal CecilArgumentsResolver(IDictionary<string, TypeReferencePair> dictionary) {
            _dictionary = new ConcurrentDictionary<string, TypeReferencePair>(dictionary);
        }

        public Type ResolveType(string argument) {
            TypeReferencePair typePair;

            if (TryResolvePair(argument, out typePair)) {
                return typePair.Type;
            }

            return null;
        }

        private bool TryResolvePair(string argument, out TypeReferencePair pair) {
            pair = null;

            if (_dictionary.TryGetValue(argument, out pair)) {
                return true;
            }

            return false;
        }

        public TypeReference ResolveTypeReference(string argument) {
            TypeReferencePair typePair;

            if (TryResolvePair(argument, out typePair)) {
                return typePair.TypeReference;
            }

            return null;
        }

        public TypeReferencePair ResolvePair(string argument) {
            TypeReferencePair typePair;

            TryResolvePair(argument, out typePair);

            return typePair;
        }

        public bool Contains(string argument) {
            return _dictionary.ContainsKey(argument);
        }
    }
}