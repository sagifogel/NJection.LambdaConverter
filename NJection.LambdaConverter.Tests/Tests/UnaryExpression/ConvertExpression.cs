using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.UnaryExpression
{
    /// <summary>
    /// Summary description for ConvertExpression
    /// </summary>
    [TestClass]
    public class ConvertExpression : BaseTest
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
        public void ConvertExpression_FromDoubleToInt_ReturnsTheCorrectValue()
        {
            Func<int> @delegate = ConvertFromDoubleToInt;
			var func = ExecuteLambda<Func<int>>(@delegate);
            var result = func();

            Assert.AreEqual(result, 10);
        }

        [TestMethod]
        public void ConvertExpression_FromMaxValueOfDoubleToInt_CausesOverflowAndReturnsTheOverlowedValue()
        {
            Func<int> @delegate = ConvertFromMaxValueOfDoubleToIntWith;
			var func = ExecuteLambda<Func<int>>(@delegate);
            var result = func();

            Assert.AreEqual(result, int.MinValue);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void ConvertExpression_FromMaxValueOfDoubleToIntWithCheckedStatment_CausesOverflowAndRaisesAnOverlowException()
        {
            Func<int> @delegate = ConvertFromMaxValueOfDoubleToIntWithCheckedStatment;
			var func = ExecuteLambda<Func<int>>(@delegate);
            var result = func();

            Assert.AreEqual(result, int.MinValue);
        }

        private int ConvertFromDoubleToInt()
        {
            double value = 10d;

            return (int)value;
        }

        private int ConvertFromMaxValueOfDoubleToIntWith()
        {
            int returnValue = 0;
            double value = double.MaxValue;

            returnValue = (int)value;

            return returnValue;
        }

        private int ConvertFromMaxValueOfDoubleToIntWithCheckedStatment()
        {
            int returnValue = 0;
            double value = double.MaxValue;

            checked
            {
                returnValue = (int)value;
            }

            return returnValue;
        }
    }
}
