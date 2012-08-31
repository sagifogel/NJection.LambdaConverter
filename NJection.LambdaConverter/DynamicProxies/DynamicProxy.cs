using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using System.Reflection;
using NRefactory = ICSharpCode.NRefactory.CSharp;
using NJection.LambdaConverter.Extensions;

namespace NJection.LambdaConverter.DynamicProxies
{
    internal class DynamicProxy
    {
        private IList<MappedInvocation> _mappedInvocations = null;

        internal DynamicProxy(Type proxiedType, Type fromType, IList<MappedInvocation> mappedInvocations)
            : this(fromType) {

            ProxiedType = proxiedType;
            _mappedInvocations = mappedInvocations;
        }

        private DynamicProxy(Type fromType) {
            FromType = ProxiedType = fromType;
            _mappedInvocations = new List<MappedInvocation>();
        }

        public Type FromType { get; private set; }

        public Type ProxiedType { get; private set; }

        internal IEnumerable<Parameter> GetOutParameters(NRefactory.ConstructorDeclaration ctor) {
            var mapped = _mappedInvocations.FirstOrDefault(m => m.CanMapTo(ctor));

            if (mapped != null) {
                return mapped.OutParameters;
            }

            return null;
        }

        public static DynamicProxy Wrap(Type type) {
            return new DynamicProxy(type);
        }
    }
}
