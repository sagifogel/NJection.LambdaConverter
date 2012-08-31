using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.BinaryOperations
{
    /// <summary>
    /// Summary description for Assign
    /// </summary>
    [TestClass]
    public class AssignExpression : BaseTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
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
        public void AssignExpression_OfNullToObject_ReturnsNull()
        {
			Func<object> @delegate = AssignNull;
            var func = ExecuteLambda<Func<object>>(@delegate);
            var @object = func();

            Assert.IsNull(@object);
        }

        [TestMethod]
        public void AssignExpression_OfIntegerToObject_BoxesTheResultAndReturnsObject()
        {
			Func<object> @delegate = BoxAndAssign;
            var func = ExecuteLambda<Func<object>>(@delegate);
            var @object = func();

            Assert.AreEqual(@object, 10);
        }

        [TestMethod]
        public void AssignExpression_OfIntegerToDouble_ConvertsTheResultAndReturnsDouble()
        {
			Func<double> @delegate = ConvertAndAssign;
            var func = ExecuteLambda<Func<double>>(@delegate);
            var result = func();

            Assert.AreEqual(result, 10);
        }

        private object AssignNull()
        {
            object @object = null;

            return @object;
        }

        private object BoxAndAssign()
        {
            object @object = new object();
            
            @object = 10;
            @object.ToString();

            return @object;
        }

        private double ConvertAndAssign()
        {
            double i = 0;
            int j = 10;

            i = j;

            return i;
        }
    }
}
