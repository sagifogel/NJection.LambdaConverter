using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJection.LambdaConverter.Tests;

namespace ICSharpCode.MethodCompiler.Tests.BinaryOperations
{
    /// <summary>
    /// Summary description for EqualsExpression
    /// </summary>
    [TestClass]
    public class EqualsExpression : BaseTest
    {
        private TestContext testContextInstance;

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
        public void EqualityExpression_UsingEqualsMethodInvocation_ReturnsTrue() {
            Func<bool> @delegate = EqualityUsingEqualsInvocation;
            var func = ExecuteLambda<Func<bool>>(@delegate);
            var result = func();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EqualityExpression_UsingOperatorOverloading_ReturnsTrue() {
            Func<bool> @delegate = EqualityUsingOperatorOverloading;
            var func = ExecuteLambda<Func<bool>>(@delegate);
            var result = func();

            Assert.IsFalse(result);
        }

        private static bool EqualityUsingEqualsInvocation() {
            object i = "Hello World";
            string s = "Hello";

            s += " World";

            return s.Equals(i);
        }


        private static bool EqualityUsingOperatorOverloading() {
            object i = "Hello World";
            string s = "Hello";

            s += " World";

            #pragma warning disable
            return s == i;
        }
    }
}
