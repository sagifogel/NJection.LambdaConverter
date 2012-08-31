using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.BinaryOperations
{
    /// <summary>
    /// Summary description for ExclusiveOrExpression
    /// </summary>
    [TestClass]
    public class ExclusiveOrExpression : BaseTest
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
        public void XorOperation_OfIntegersBothWithMaxValue_ReturnsZero()
        {
			Func<int> @delegate = Xor;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();

            Assert.AreEqual(result, 0);
        }

        [TestMethod]
        public void XorAssignOperation_OfIntegersBothWithMaxValue_ReturnsZero()
        {
			Func<int> @delegate = XorAssign;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();

            Assert.AreEqual(result, 0);
        }
        
        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void XorOperation_OfByteAndIntegerThatCausesAnOverflowWithCheck_RaisesAnOverflowException()
        {
			Func<int> @delegate = XorMaxValueWithChecked;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();
        }

        [TestMethod]
        public void XorOperation_OfByteAndIntegerThatCausesAnOverflowWithoutCheck_CausesAnOverflowAndReturnsTheOverflowValue()
        {
			Func<int> @delegate = XorMaxValueWithoutChecked;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();

            Assert.AreEqual(result, 0);
        }

        private int Xor()
        {
            int i = int.MaxValue;
            int maxValue = int.MaxValue;

            return i ^ maxValue;
        }

        private int XorAssign()
        {
            int i = int.MaxValue;
            int maxValue = int.MaxValue;

            i ^= maxValue;

            return i;
        }

        private int XorMaxValueWithChecked()
        {
            byte i = byte.MaxValue;
            int j = int.MaxValue;

            checked
            {
                i = (byte)(i ^ j);
            }

            return i;
        }

        private int XorMaxValueWithoutChecked()
        {
            byte i = byte.MaxValue;
            int j = int.MaxValue;

            unchecked
            {
                i = (byte)(i ^ j);
            }

            return i;
        }
    }
}
