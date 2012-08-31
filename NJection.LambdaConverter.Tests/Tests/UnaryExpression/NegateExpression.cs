using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.UnaryExpression
{
    /// <summary>
    /// Summary description for Negate
    /// </summary>
    [TestClass]
    public class NegateExpression : BaseTest
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
        public void NegateExpresion_GivenASpecificValue_ReturnsTheNegatedValue()
        {
            int toBeNegated = 10;
			Func<int, int> @delegate = Negate;
			var func = ExecuteLambda<Func<int, int>>(@delegate);
            var result = func(toBeNegated);

            Assert.AreEqual(toBeNegated, result * -1);
        }

        [TestMethod]
        public void DoubleNegationExpresion_GivenASpecificValue_ReturnsTheOriginalValue()
        {
            int value = 10;
			Func<int, int> @delegate = DoubleNegation;
			var func = ExecuteLambda<Func<int, int>>(@delegate);
            var result = func(value);

            Assert.AreEqual(result, value);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void NegateExpresion_OfLongWithValueOfMaxValueWhenNegatedAndConvertedToIntInsideCheckedStatement_RaisesAnOverflowException()
        {
			Func<int> @delegate = NegateMaxValueWithChecked;
			var func = ExecuteLambda<Func<int>>(@delegate);
            var result = func();
        }

        [TestMethod]
        public void NegateExpresion_OfLongWithAvalueOfMaxValueWhenNegatedAndConvertedToInt_CausesOverflowAndReturnsTheCorrectNegatedOverflowedValue()
        {
			Func<int> @delegate = NegateMaxValueWithoutChecked;
			var func = ExecuteLambda<Func<int>>(@delegate);
            var result = func();

            Assert.AreEqual(result, 1);
        }

        private static int Negate(int i)
        {
            return -i;
        }

        private static int DoubleNegation(int i)
        {
            return -(-i);
        }

        private static int NegateMaxValueWithChecked()
        {
            int i = 0;
            long l = long.MaxValue;

            checked
            {
                i = (int)-l;
            }

            return i;
        }

        private static int NegateMaxValueWithoutChecked()
        {
            int i = 0;
            long l = long.MaxValue;

            i = (int)-l;

            return i;
        }
    }
}
