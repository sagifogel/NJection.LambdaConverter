using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.BinaryOperations
{
    [TestClass]
    public class SubtractExpression : BaseTest
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
        public void SubtractOperation_OfIntegers_ReturnsTheCorrectSubtractedNumber()
        {
			Func<int> @delegate = Subtract;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();

            Assert.AreEqual(result, 0);
        }

        [TestMethod]
        public void SubtractAssignOperation_OfIntegers_ReturnsTheCorrectSubtractedNumber()
        {
			Func<int> @delegate = SubtractAssign;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();

            Assert.AreEqual(result, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void SubtractCheckedOperation_OfIntegersThatOneOfThemHasMinValue_RaisesAnOverflowException()
        {
			Func<int> @delegate = SubtractMinValueWithChecked;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();
        }

        [TestMethod]
        public void SubtractChecked_OfIntegersThatOneOfThemHasMinValue_CausesOverflowAndReturnsTheCorrectSubtractedNumber()
        {
            int additional = 10;
			Func<int, int> @delegate = SubtractMinValueWithoutCheck;
			var func = ExecuteLambda<Func<int, int>>(@delegate);
            int result = func(additional);

            additional -= 1;
            Assert.AreEqual(result, int.MaxValue - additional);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void SubtractAssginCheckedOperation_OfIntegersThatOneOfThemHasMinValue_RaisesAnOverflowException()
        {
			Func<int> @delegate = SubtractAssignChecked;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();
        }

        private int Subtract()
        {
            int i = 10;
            int j = 10;

            return i - j;
        }

        private int SubtractMinValueWithChecked()
        {
            int i = int.MinValue;
            int j = 10;

            checked
            {
                j = j - i;
            }

            return j;
        }

        private int SubtractMinValueWithoutCheck(int additional)
        {
            int i = int.MinValue;

            return i - additional;
        }

        private int SubtractAssign()
        {
            int i = 10;
            int j = 10;

            j -= i;

            return j;
        }

        private int SubtractAssignChecked()
        {
            int i = int.MinValue;
            int j = 10;

            checked
            {
                j -= i;
            }

            return j;
        }
    }
}
