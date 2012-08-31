using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.BinaryOperations
{
    /// <summary>
    /// Summary description for And
    /// </summary>
    [TestClass]
    public class AndExpression : BaseTest
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
        public void AndAlsoOperation_WithSecondConditionThatRaisesANullReferenceExceptionWontBeCalledBecauseFirstConditionEvaluatesToFalse_ReturnsFalse()
        {
            Func<bool> @delegate = AndAlso;
			var func = ExecuteLambda<Func<bool>>(@delegate);
            var result = func();

            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void BitwiseAndOperation_GivenSpecificValuesThatHaveCommonBitsThatAreSet_ReturnsTheResultOfABitMaskingOperation()
        {
            int i = 3, j = 2;
            Func<int, int, int> @delegate = BitwiseAnd;
			var func = ExecuteLambda<Func<int, int, int>>(@delegate);
            var result = func(i, j);

            Assert.AreEqual(result, j);
        }

        [TestMethod]
        public void BitwiseAndAssignOperation_GivenSpecificValuesThatHaveCommonBitsThatAreSet_ReturnsTheResultOfABitMaskingOperation()
        {
            int i = 3, j = 2;
            Func<int, int, int> @delegate = BitwiseAndAssign;
			var func = ExecuteLambda<Func<int, int, int>>(@delegate);
            var result = func(i, j);

            Assert.AreEqual(result, j);
        }

        private bool AndAlso()
        {
            object @object = null;

            return @object != null && @object.ToString().Equals(string.Empty);
        }

        private int BitwiseAnd(int i, int j)
        {
            return i & j;
        }

        private int BitwiseAndAssign(int i, int j)
        {
            j &= i;

            return j;
        }
    }
}
