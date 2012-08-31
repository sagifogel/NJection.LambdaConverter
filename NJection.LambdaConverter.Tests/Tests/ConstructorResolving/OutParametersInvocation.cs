using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.ConstructorResolving
{
    [TestClass]
    public class OutParametersInvocation : BaseConstructorTest
    {
        private delegate DerivedClass CtorUsingOutParam(ref int i);

        private TestContext testContextInstance;

        public class DerivedClass : BaseClass
        {
            public string MyProperty { get; set; }

            public DerivedClass(int value)
                : base(ref value) {

                MyProperty = string.Empty;
            }

            public DerivedClass(ref int value)
                : base(ref value) {
                MyProperty = string.Empty;
            }
        }

        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        [TestMethod]
        public void ConstructorResolving_UsingOutParameterChangeTheValueOfTheSentArgumentAndReturnsProxiedInstanceWithChangedValue() {
            int argument = 10;
            int originalValue = argument;
            var ctor = GetConstructorByType<DerivedClass>(typeof(int).MakeByRefType());
            var func = ExecuteConstructorLambda<CtorUsingOutParam>(ctor);
            var derivedClass = func(ref argument);
            int baseValue = (int)derivedClass.BaseValue;

            Assert.AreNotEqual(originalValue, argument);
            Assert.AreEqual(baseValue, 100);
        }

        [TestMethod]
        public void ConstructorResolving_UsingOutParameterInBaseInvocation_DoNotChangeTheValueOfTheSentArgumentAndReturnsProxiedInstanceWithChangedValue() {
            int argument = 10;
            int originalValue = argument;
            var ctor = GetConstructorByType<DerivedClass>(typeof(int));
            var func = ExecuteConstructorLambda<Func<int, DerivedClass>>(ctor);
            var derivedClass = func(argument);
            int baseValue = (int)derivedClass.BaseValue;

            Assert.AreEqual(originalValue, argument);
            Assert.AreEqual(baseValue, 100);
        }
    }
}
