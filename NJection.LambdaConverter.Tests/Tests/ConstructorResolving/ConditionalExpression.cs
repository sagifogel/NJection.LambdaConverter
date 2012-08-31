using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.ConstructorResolving
{
    [TestClass]
    public class ConditionalExpression : BaseConstructorTest
    {
        private TestContext testContextInstance;

        public class DerivedClass : BaseClass
        {
            public string MyProperty { get; set; }

            public DerivedClass(bool b)
                : base(b ? new object() : string.Empty) {
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
        #endregion

        [TestMethod]
        public void ConstructorResolving_UsingConditionalExpressionForBaseConstructorWhenEvaluatesToTrue_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(bool));
            var func = ExecuteConstructorLambda<Func<bool, DerivedClass>>(ctor);
            var derivedClass = func(true);

            Assert.IsNotInstanceOfType(derivedClass.BaseValue, typeof(string));
        }

        [TestMethod]
        public void ConstructorResolving_UsingConditionalExpressionForBaseConstructorWhenEvaluatesToFalse_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(bool));
            var func = ExecuteConstructorLambda<Func<bool, DerivedClass>>(ctor);
            var derivedClass = func(false);

            Assert.IsInstanceOfType(derivedClass.BaseValue, typeof(string));
        }
    }
}
