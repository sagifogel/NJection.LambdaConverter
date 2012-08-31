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

        static AssemblyResolver() {
            _instance = new Lazy<AssemblyResolver>(() => new AssemblyResolver(), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        private AssemblyResolver() {
            _assemblies = AppDomain.CurrentDomain
                                   .GetAssemblies()
                                   .ToThreadSafeDictionary(a => a.GetName().Name);
        }

        public static AssemblyResolver Instance {
            get { return _instance.Value; }
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name) {
            return Resolve(name.Name);
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters) {
            return Resolve(name.Name);
        }

        public AssemblyDefinition Resolve(Assembly assembly) {
            AssemblyDefinition assemblyDefinition;

            TryResolveDefinition(assembly.GetName().Name, out assemblyDefinition);
            return assemblyDefinition;
        }

        public AssemblyDefinition Resolve(string assemblyName) {
            AssemblyDefinition assembly;

            TryResolveDefinition(assemblyName, out assembly);
            return assembly;
        }

        public bool TryResolveDefinition(string assemblyName, out AssemblyDefinition assemblyDefinition) {
            Assembly assembly;

            if (TryResolve(assemblyName, out assembly)) {
                string fullyQualifiedName = assembly.ManifestModule.FullyQualifiedName;

                assemblyDefinition = AssemblyDefinition.ReadAssembly(fullyQualifiedName);
                return true;
            }

            assemblyDefinition = null;
            return false;
        }

        public bool TryResolve(string assemblyName, out Assembly assembly) {
            return _assemblies.TryGetValue(assemblyName, out assembly);
        }

        public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters) {
            return Resolve(fullName.Split(',')[0]);
        }
    }
}