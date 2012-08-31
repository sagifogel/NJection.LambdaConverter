using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.BinaryOperations
{
    [TestClass]
    public class AddExpression : BaseTest
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
        public void AddOperation_OfIntegers_ReturnsTheCorrectAdditionedNumber()
        {   
            Func<int> @delegate = Add;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();

            Assert.AreEqual(result, 20);
        }

        [TestMethod]
        public void AddAssignOperation_OfIntegers_ReturnsTheCorrectAdditionedNumber()
        {
            Func<int> @delegate = AddAssign;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();

            Assert.AreEqual(result, 20);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void AddCheckedOperation_OfIntegersThatOneOfThemHasMaxValue_RaisesAnOverflowException()
        {
            Func<int> @delegate = AddMaxValueWithChecked;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();
        }

        [TestMethod]
        public void AddChecked_OfIntegersThatOneOfThemHasMaxValue_CausesOverflowAndReturnsTheCorrectAdditionedNumber()
        {
            int additional = 10;
            Func<int, int> @delegate = AddMaxValueWithoutCheck;
			var func = ExecuteLambda<Func<int, int>>(@delegate);
            int result = func(additional);

            additional -= 1;
            Assert.AreEqual(result, int.MinValue + additional);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void AddAssginCheckedOperation_OfIntegersThatOneOfThemHasMaxValue_RaisesAnOverflowException()
        {
            Func<int> @delegate = AddAssignChecked;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();
        }

        [TestMethod]
        public void AddOperation_OfTwoStrings_ReturnsConcatenatedString()
        {
            Func<string> @delegate = AddOfStrings;
			var func = ExecuteLambda<Func<string>>(@delegate);
            string result = func();

            Assert.IsTrue(result.Equals("Hello World", StringComparison.OrdinalIgnoreCase));
        }

       private int Add()
        {
            int i = 10;
            int j = 10;

            return i + j;
        }

        private int AddMaxValueWithChecked()
        {
            int i = int.MaxValue;
            int j = 10;

            checked
            {
                j = j + i;
            }

            return j;
        }

        private int AddMaxValueWithoutCheck(int additional)
        {
            int i = int.MaxValue;
            
            return i + additional;
        }

        private string AddOfStrings()
        {
            string i = "Hello";
            string j = "World";

            return i + " " + j;
        }

        private int AddAssign()
        {
            int i = 10;
            int j = 10;

            j += i;

            return j;
        }

        private int AddAssignChecked()
        {
            int i = int.MaxValue;
            int j = 10;

            checked
            {
                j += i;
            }

            return j;
        }
    }
}
