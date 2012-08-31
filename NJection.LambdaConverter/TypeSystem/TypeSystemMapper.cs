using System;
using System.Collections.Generic;

namespace NJection.LambdaConverter.Mappers
{
    internal static class TypeSystemMapper
    {
        private static Dictionary<string, Type> _commonTypeSystem = null;

        static TypeSystemMapper() {
            _commonTypeSystem = new Dictionary<string, Type>
            {
			    { "bool", TypeSystem.Boolean },
                { "byte", TypeSystem.Byte },
                { "char", TypeSystem.Char },
			    { "double", TypeSystem.Double },
			    { "short", TypeSystem.Short },
			    { "int", TypeSystem.Int },
			    { "long", TypeSystem.Long },
			    { "object", TypeSystem.Object },
			    { "sbyte", TypeSystem.Sbyte },
			    { "float", TypeSystem.Float },
			    { "string", TypeSystem.String },
			    { "ushort", TypeSystem.Ushort },
			    { "uint", TypeSystem.Uint },
			    { "ulong", TypeSystem.Ulong },
			    { "void", TypeSystem.Void },
			    { "dynamic", TypeSystem.Object },
                { "decimal", TypeSystem.Decimal }
            };
        }

        internal static Type Map(string key) {
            return _commonTypeSystem[key];
        }
    }
}