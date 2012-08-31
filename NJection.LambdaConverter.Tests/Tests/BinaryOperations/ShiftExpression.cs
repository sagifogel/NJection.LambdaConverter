using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.BinaryOperations
{
    /// <summary>
    /// Summary description for ShiftExpression
    /// </summary>
    [TestClass]
    public class ShiftExpression : BaseTest
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
        public void LeftShiftExpression_GivenASpecificNumberWhenShiftedToTheLeftByOne_ReturnsTheValueMultipliedByTwo()
        {   
            var argument = 4;
			Func<int, int> @delegate = LeftShift;
			var func = ExecuteLambda<Func<int, int>>(@delegate);
            var result = func(argument);

            Assert.AreEqual(result, argument * 2);
        }

        [TestMethod]
        public void LeftShiftAssignExpression_GivenASpecificNumberWhenShiftedToTheLeftByOne_ReturnsTheValueMultipliedByTwo()
        {
            var argument = 4;
			Func<int, int> @delegate = LeftShiftAssign;
			var func = ExecuteLambda<Func<int, int>>(@delegate);
            var result = func(argument);

            Assert.AreEqual(result, argument * 2);
        }

        [TestMethod]
        public void RightShiftExpression_GivenASpecificNumberWhenShiftedToTheRightByOne_ReturnsTheValueDividedByTwo()
        {
            var argument = 4;
			Func<int, int> @delegate = RightShift;
			var func = ExecuteLambda<Func<int, int>>(@delegate);
            var result = func(argument);

            Assert.AreEqual(result, argument / 2);
        }

        [TestMethod]
        public void RightShiftAssignExpression_GivenASpecificNumberWhenShiftedToTheRightByOne_ReturnsTheValueDividedByTwo()
        {
            var argument = 4;
			Func<int, int> @delegate = RightShiftAssign;
			var func = ExecuteLambda<Func<int, int>>(@delegate);
            var result = func(argument);

            Assert.AreEqual(result, argument / 2);
        }

        private int LeftShift(int i)
        {
            i = i << 1;

            return i;
        }

        private int LeftShiftAssign(int i)
        {
            i <<= 1;

            return i;
        }

        private int RightShift(int i)
        {
            i = i >> 1;

            return i;
        }

        private int RightShiftAssign(int i)
        {
            i >>= 1;

            return i;
        }

        private int LeftShiftChecked(int i)
        {
            checked
            {
                i <<= 1;
            }

            return i;
        }
    }
}
