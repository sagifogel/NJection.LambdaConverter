using System;
using System.Reflection.Emit;
using System.Threading;

namespace NJection.LambdaConverter.DynamicProxies
{
    internal sealed class DynamicModuleBuilder
    {
        private static readonly Lazy<ModuleBuilder> _moduleBuilder = null;

        private DynamicModuleBuilder() { }

        static DynamicModuleBuilder() {
            _moduleBuilder = new Lazy<ModuleBuilder>(CreateAssemblyBuilder, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        internal static ModuleBuilder Instance {
            get { return _moduleBuilder.Value; }
        }

        private static ModuleBuilder CreateAssemblyBuilder() {
            var assemblyBuilder = DynamicAssemblyBuilder.Instance;
            string assemblyName = string.Format("{0}.dll", DynamicAssemblyBuilder.AssemblyName);

            return assemblyBuilder.DefineDynamicModule(assemblyName, false);
        }
    }
}