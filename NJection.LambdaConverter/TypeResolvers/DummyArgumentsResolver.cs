using System;
using System.Threading;
using Mono.Cecil;

namespace NJection.LambdaConverter.TypeResolvers
{
    internal sealed class DummyArgumentsResolver : ICecilArgumentsResolver
    {
        private static readonly Lazy<DummyArgumentsResolver> _instance = null;

        static DummyArgumentsResolver() {
            _instance = new Lazy<DummyArgumentsResolver>(() => new DummyArgumentsResolver(), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        private DummyArgumentsResolver() { }

        internal static DummyArgumentsResolver Instance {
            get { return _instance.Value; }
        }

        public bool Contains(string argument) {
            return false;
        }

        public Type ResolveType(string argument) {
            return null;
        }

        public TypeReferencePair ResolvePair(string argument) {
            return null;
        }

        public TypeReference ResolveTypeReference(string argument) {
            return null;
        }
    }
}