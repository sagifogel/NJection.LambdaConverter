using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;

namespace NJection.LambdaConverter.DynamicProxies
{
    public class Parameter
    {   
        public Parameter(string name, Type type, int location) {
            Type = type;
            Name = name;
            Location = location;
            IsByRef = type.IsByRef;
        }

        public Type Type { get; private set; }
        
        public string Name { get; private set; }

        public bool IsByRef { get; private set; }

        public int Location { get; private set; }
    }
}
