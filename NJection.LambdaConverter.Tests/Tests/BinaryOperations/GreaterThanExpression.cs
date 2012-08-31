using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.BinaryOperations
{
    /// <summary>
    /// Summary description for GreaterThan
    /// </summary>
    [TestClass]
    public class GreaterThanExpression : BaseTest
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
        public void GreaterThanExpression_WithALeftOperandThatIsGreaterThanTheRightOperand_ReturnsTrue()
        {
			Func<bool> @delegate = GreaterThan;
			var func = ExecuteLambda<Func<bool>>(@delegate);
            bool result = func();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GreaterThanOrEqualExpression_WithLeftOperandThatIsEqualToTheRightOperand_ReturnsTrue()
        {
			Func<bool> @delegate = GreaterThanOrEqual;
			var func = ExecuteLambda<Func<bool>>(@delegate);
            bool result = func();

            Assert.IsTrue(result);
        }

        private bool GreaterThan()
        {
            int i = 100;

            return i > 90;
        }

        private bool GreaterThanOrEqual()
        {
            int i = 100;

            return i >= 100;
        }
    }
}
