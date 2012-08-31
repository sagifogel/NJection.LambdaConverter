using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.ConstructorResolving
{
    [TestClass]
    public class PrimitiveParameters : BaseConstructorTest
    {
        private TestContext testContextInstance;

        public class DerivedClass : BaseClass
        {
            public string _value = null;

            public DerivedClass()
                : base("literal") {
                _value = string.Empty;
            }

            public DerivedClass(string value)
                : base(value) {
                _value = value;
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

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion Additional test attributes

        [TestMethod]
        public void ConstructorResolving_UsingStringLiteralAsAnArgumentForBaseConstructor_ReturnsProxiedInstance() {
            var ctor = GetConstructorByType<DerivedClass>(Type.EmptyTypes);
            var func = ExecuteConstructorLambda<Func<DerivedClass>>(ctor);
            var derivedClass = func();

            Assert.AreEqual(derivedClass.BaseValue, "literal");
        }

        [TestMethod]
        public void ConstructorResolving_UsingAnArgumentForBaseConstructor_ReturnsProxiedInstance() {
            string argument = "argument";
            var ctor = GetConstructorByType<DerivedClass>(typeof(string));
            var func = ExecuteConstructorLambda<Func<string, DerivedClass>>(ctor);
            var derivedClass = func(argument);

            Assert.AreEqual(derivedClass.BaseValue, argument);
        }
    }
}