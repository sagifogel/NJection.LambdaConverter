using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.ConstructorResolving
{
    [TestClass]
    public class DefaultValue : BaseConstructorTest
    {
        private TestContext testContextInstance;

        public struct DummyStruct
        {
        }

        public class DerivedClass : BaseClass
        {
            public int Value { get; set; }

            public DerivedClass(DummyStruct s)
                : base(default(DummyStruct)) {
                Value = 0;
            }

            public DerivedClass()
                : base(new DummyStruct()) {
                Value = 0;
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
        public void ConstructorResolving_UsingCreateNewStructArgumentUsingDefaultExpressionForBaseConstructor_ReturnsProxiedInstanceWithNewStruct() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(DummyStruct));
            var func = ExecuteConstructorLambda<Func<DummyStruct, DerivedClass>>(ctor);
            var derivedClass = func(default(DummyStruct));
            var dummy = derivedClass.BaseValue;

            Assert.IsInstanceOfType(dummy, typeof(DummyStruct));
        }

        [TestMethod]
        public void ConstructorResolving_UsingCreateNewStructArgumentWithoutIntializersAndWithoutArgumentsForBaseConstructor_ReturnsProxiedInstanceWithNewStruct() {
            var ctor = GetConstructorByType<DerivedClass>(Type.EmptyTypes);
            var func = ExecuteConstructorLambda<Func<DerivedClass>>(ctor);
            var derivedClass = func();
            var dummy = derivedClass.BaseValue;

            Assert.IsInstanceOfType(dummy, typeof(DummyStruct));
        }
    }
}