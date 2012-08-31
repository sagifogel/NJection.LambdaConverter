using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.ConstructorResolving
{
    /// <summary>
    /// Summary description for CastExpression
    /// </summary>
    [TestClass]
    public class CastExpression : BaseConstructorTest
    {
        private TestContext testContextInstance;

        public class DerivedClass : BaseClass
        {
            public string MyProperty { get; set; }

            public DerivedClass(int value)
                : base((ValueType)value) {
                MyProperty = string.Empty;
            }

            public DerivedClass(object value)
                : base(value as Exception) {
                MyProperty = string.Empty;
            }

            public DerivedClass(Exception value)
                : base(value is StackOverflowException) {
                MyProperty = string.Empty;
            }
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        [TestMethod]
        public void ConstructorResolving_UsingValueTypeCastingForBaseConstructor_ReturnsProxiedInstanceWithCastedType() {
            int argument = 10;
            var ctor = GetConstructorByType<DerivedClass>(typeof(int));
            var func = ExecuteConstructorLambda<Func<int, DerivedClass>>(ctor);
            var derivedClass = func(argument);

            Assert.AreEqual(derivedClass.BaseValue, argument);
        }

        [TestMethod]
        public void ConstructorResolving_UsingAsExpressionCastingForBaseConstructor_ReturnsProxiedInstanceWithCastedType() {
            var exception = new Exception();
            var ctor = GetConstructorByType<DerivedClass>(typeof(object));
            var func = ExecuteConstructorLambda<Func<object, DerivedClass>>(ctor);
            var derivedClass = func(exception);
            var result = derivedClass.BaseValue as Exception;

            Assert.IsInstanceOfType(derivedClass.BaseValue, typeof(Exception));
        }

        [TestMethod]
        public void ConstructorResolving_UsingIsExpressionCastingForBaseConstructor_ReturnsProxiedInstanceWithCastedType() {
            var exception = new DivideByZeroException();
            var ctor = GetConstructorByType<DerivedClass>(typeof(Exception));
            var func = ExecuteConstructorLambda<Func<Exception, DerivedClass>>(ctor);
            var derivedClass = func(exception);
            var result = (bool)derivedClass.BaseValue;

            Assert.IsFalse(result);
        }
    }
}