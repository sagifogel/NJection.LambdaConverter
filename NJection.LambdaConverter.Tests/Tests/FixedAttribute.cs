using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NJection.LambdaConverter.Tests
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class FixedAttribute : Attribute
    {
        public FixedAttribute(string fix) {

        }
    }
}
