using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using Mono.Cecil;
using NJection.LambdaConverter.Extensions;

namespace NJection.LambdaConverter
{
    internal sealed class AssemblyResolver : IAssemblyResolver
    {
        private static readonly Lazy<AssemblyResolver> _instance = null;
        private readonly ConcurrentDictionary<string, Assembly> _assemblies = null;
        private readonly ConcurrentDictionary<string, AssemblyDefinition> _assembliesDef = null;

        static AssemblyResolver() {
            _instance = new Lazy<AssemblyResolver>(() => new AssemblyResolver(), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        private AssemblyResolver() {
            _assemblies = AppDomain.CurrentDomain
                                   .GetAssemblies()
                                   .ToThreadSafeDictionary(a => a.GetName().FullName);
            _assembliesDef = new ConcurrentDictionary<string, AssemblyDefinition>();
        }

        public static AssemblyResolver Instance {
            get { return _instance.Value; }
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name) {
            return Resolve(name.FullName);
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters) {
            return Resolve(name.FullName);
        }

        public AssemblyDefinition Resolve(Assembly assembly) {
            AssemblyDefinition assemblyDefinition;

            TryResolveDefinition(assembly.GetName().FullName, out assemblyDefinition);
            return assemblyDefinition;
        }

        public AssemblyDefinition Resolve(string assemblyName) {
            AssemblyDefinition assembly;

            TryResolveDefinition(assemblyName, out assembly);
            return assembly;
        }

        public bool TryResolveDefinition(string assemblyName, out AssemblyDefinition assemblyDefinition) {
            Assembly assembly;

            if (!_assembliesDef.TryGetValue(assemblyName, out assemblyDefinition)) {
                if (TryResolve(assemblyName, out assembly)) {
                    string fullyQualifiedName = assembly.ManifestModule.FullyQualifiedName;
                    assemblyDefinition = AssemblyDefinition.ReadAssembly(fullyQualifiedName);
                    _assembliesDef.TryAdd(assemblyName, assemblyDefinition);
                }
            }

            return assemblyDefinition != null;
        }

        public bool TryResolve(string assemblyName, out Assembly assembly) {
            return _assemblies.TryGetValue(assemblyName, out assembly);
        }

        public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters) {
            return Resolve(fullName.Split(',')[0]);
        }
    }
}