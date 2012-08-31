using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.BinaryOperations
{
    /// <summary>
    /// Summary description for OrExpression
    /// </summary>
    [TestClass]
    public class OrExpression : BaseTest
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
        public void OrAlsoExpression_WithSecondConditionThatRaisesNullReferenceExceptionWontBeCalledBecauseTheFirstConditionEvaluatesToTrue_ReturnTrue()
        {
			Func<bool> @delegate = OrAlso;
			var func = ExecuteLambda<Func<bool>>(@delegate);
            var result = func();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void BitwiseOrExpression_GivenSpecificValues_ReturnsTheCorrectValue()
        {
            int i = 16, j = 4;
			Func<int, int, int> @delegate = BitwiseOr;
			var func = ExecuteLambda<Func<int, int, int>>(@delegate);
            var result = func(i, j);

            Assert.AreEqual(result, 20);
        }

        [TestMethod]
        public void BitwiseOrAssignExpression_GivenSpecificValues_ReturnsTheCorrectValue()
        {
            int i = 16, j = 4;
			Func<int, int, int> @delegate = BitwiseOrAssign;
			var func = ExecuteLambda<Func<int, int, int>>(@delegate);
            var result = func(i, j);

            Assert.AreEqual(result, 20);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void BitwiseOrExpression_OfByteWithValueOfMaxValueAndIntegerWhichItsValueIsGreaterThanByteMaxValue_CausesOverflowAndRaisesAnException()
        {
			Func<byte> @delegate = BitwiseOrChecked;
			var func = ExecuteLambda<Func<byte>>(@delegate);
            var result = func();
        }

        [TestMethod]
        public void BitwiseOrExpression_OfByteWithValueOfMaxValueAndIntegerWhichItsValueIsGreaterThanByteMaxValue_CausesOverflowAndReturnsTheOverflowedValue()
        {
			Func<byte> @delegate = BitwiseOrUnchecked;
			var func = ExecuteLambda<Func<byte>>(@delegate);
            var result = func();

            Assert.AreEqual(result, 255);
        }

        private bool OrAlso()
        {
            object @object = null;

            return @object == null || @object.ToString().Equals(string.Empty);
        }

        private int BitwiseOr(int i, int j)
        {
            return i | j;
        }

        private int BitwiseOrAssign(int i, int j)
        {
            j |= i;

            return j;
        }

        private byte BitwiseOrChecked()
        {
            int i = 1000;
            byte j = byte.MaxValue;

            checked
            {
                j = (byte)(i | j); 
            }

            return j;
        }

        private byte BitwiseOrUnchecked()
        {
            int i = 1000;
            byte j = byte.MaxValue;

            unchecked
            {
                j = (byte)(i | j); 
            }

            return j;
        }
    }
}
