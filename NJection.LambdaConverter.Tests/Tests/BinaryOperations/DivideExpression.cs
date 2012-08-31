using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.BinaryOperations
{
    /// <summary>
    /// Summary description for Divide
    /// </summary>
    [TestClass]
    public class DivideExpression : BaseTest
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
        public void DividenOperation_OfIntegers_ReturnsTheCorrectNumber()
        {
            Func<int> @delegate = Divide;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();

            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public void DivideAssignOperation_OfIntegers_ReturnsTheCorrectNumber()
        {
            Func<int> @delegate = DivideAssign;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();

            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void DivideCheckedOperation_OfIntegersThatOneOfThemHasMaxValue_CausesOverflowAndRaisesAnOverflowException()
        {
            Func<int> @delegate = DivideMaxValueWithChecked;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();
        }

        [TestMethod]
        public void DivideAssginUncheckedOperation_OfIntegersThatOneOfThemHasMaxValue_CausesOverflowAndReturnsTheOverflowValue()
        {
            Func<int> @delegate = DivideMaxValueWithoutCheck;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();

            Assert.AreEqual(result, int.MinValue);
        }

        private int Divide()
        {
            int i = 10;
            int j = 10;

            return i / j;
        }

        private int DivideAssign()
        {
            int i = 10;
            int j = 10;

            i /= j;

            return i;
        }

        private int DivideMaxValueWithChecked()
        {
            int i = int.MaxValue;
            int result = 0;

            checked
            {
                result = (int)(i / 0.1);
            }

            return result;
        }

        private int DivideMaxValueWithoutCheck()
        {
            int i = int.MaxValue;
            int result = 0;

            unchecked
            {
                result = (int)(i / 0.1);
            }

            return result;
        }
    }
}
