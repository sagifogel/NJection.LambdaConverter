using System;
using System.Reflection;

namespace NJection.LambdaConverter.Tests.ConstructorResolving
{
    public class BaseConstructorTest : BaseTest
    {
        public class BaseClass
        {
            public object BaseValue { get; private set; }

            public BaseClass() { }

            public BaseClass(object value) {
                BaseValue = value;
            }

            public BaseClass(int value) {
                BaseValue = value;
            }

            public BaseClass(ref int value) {
                BaseValue = value = 100;
            }

            public BaseClass(bool value) {
                BaseValue = value;
            }
        }

        public ConstructorInfo GetConstructorByType<TConstructor>(params Type[] types) {
            return typeof(TConstructor).GetConstructor(types);
        }
    }
}