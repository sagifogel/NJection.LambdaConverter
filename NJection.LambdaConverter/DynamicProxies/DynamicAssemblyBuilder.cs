using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace NJection.LambdaConverter.DynamicProxies
{
    internal sealed class DynamicAssemblyBuilder
    {
        private static readonly string _assemblyName = null;
        private static readonly Lazy<AssemblyBuilder> _assemblyBuilder = null;

        private DynamicAssemblyBuilder() { }

        static DynamicAssemblyBuilder() {
            _assemblyName = string.Format("{0}.Proxies", Path.GetFileNameWithoutExtension(typeof(DynamicAssemblyBuilder).Namespace));
            _assemblyBuilder = new Lazy<AssemblyBuilder>(CreateAssemblyBuilder, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        internal static string AssemblyName {
            get { return _assemblyName; }
        }

        internal static AssemblyBuilder Instance {
            get { return _assemblyBuilder.Value; }
        }

        private static AssemblyBuilder CreateAssemblyBuilder() {
            var name = new AssemblyName(_assemblyName);

            return AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
        }
    }
}