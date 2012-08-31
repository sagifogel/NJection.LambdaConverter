using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.BinaryOperations
{
    [TestClass]
    public class MultiplyExpression : BaseTest
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
        public void MultiplyOperation_OfIntegers_ReturnsTheCorrectMultipliedNumber()
        {
			Func<int> @delegate = Multiply;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();

            Assert.AreEqual(result, 100);
        }

        [TestMethod]
        public void MultiplyAssignOperation_OfIntegers_ReturnsTheCorrectMultipliedNumber()
        {
			Func<int> @delegate = MultiplyAssign;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();

            Assert.AreEqual(result, 100);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void MultiplyCheckedOperation_OfIntegersThatOneOfThemHasMaxValue_RaisesAnOverflowException()
        {
			Func<int> @delegate = MultiplyMaxValueWithChecked;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void MultiplyAssginCheckedOperation_OfIntegersThatOneOfThemHasMaxValue_RaisesAnOverflowException()
        {
			Func<int> @delegate = MultiplyAssignChecked;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();
        }

        [TestMethod]
        public void MultiplyOperation_OfIntegersThatOneOfThemHasMaxValueWithNoCheck_CausesOverflowAndReturnsTheOverflowedValue()
        {
			Func<int> @delegate = MultiplyMaxValueWithoutCheck;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();

            Assert.AreEqual(result, -2);
        }

        private int Multiply()
        {
            int i = 10;
            int j = 10;

            return i * j;
        }

        private int MultiplyMaxValueWithChecked()
        {
            int i = int.MaxValue;
            int j = 2;

            checked
            {
                j = j * i;
            }

            return j;
        }

        private int MultiplyMaxValueWithoutCheck()
        {
            int i = int.MaxValue;
            int j = 2;

            return i * j;
        }

        private int MultiplyAssign()
        {
            int i = 10;
            int j = 10;

            j *= i;

            return j;
        }

        private int MultiplyAssignChecked()
        {
            int i = int.MaxValue;
            int j = 10;

            checked
            {
                j *= i;
            }

            return j;
        }
    }
}
