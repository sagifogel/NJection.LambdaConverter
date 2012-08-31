using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.UnaryExpression
{
    /// <summary>
    /// Summary description for DecrementExpression
    /// </summary>
    [TestClass]
    public class DecrementExpression : BaseTest
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
        public void PostDecrementAssignExpression_OfIntegerAsAnArgumentAtTheReturnStatement_DoesNotChangeTheArgumentsOriginalValue()
        {
            int i = 10;
            Func<int, int> @delegate = PostDecrementAssignInReturnStatement;
			var func = ExecuteLambda<Func<int, int>>(@delegate);
            int result = func(i);

            Assert.AreEqual(result, i);
        }

        [TestMethod]
        public void PostDecrementAssignExpression_OfIntegerBeforeTheReturnStatement_DecrementsTheArgumentsValue()
        {
            int i = 10;
            Func<int, int> @delegate = PostDecrementAssign;
			var func = ExecuteLambda<Func<int, int>>(@delegate);
            int result = func(i);

            Assert.AreEqual(result, i - 1);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void PostDecrementAssignExpression_OfIntegersThatHasMinValueWithCheckedStatement_RaisesAnOverflowException()
        {
            Func<int> @delegate = PostDecrementMinValueWithChecked;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();
        }

        [TestMethod]
        public void PostDecrementAssignExpression_OfIntegersThatHasMinValue_CausesOverflowAndReturnsTheCorrectDecrementedNumber()
        {
            Func<int> @delegate = PostDecrementMinValueWithoutCheck;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();

            Assert.AreEqual(result, int.MaxValue);
        }

        [TestMethod]
        public void PreDecrementAssignExpression_OfInteger_DecrementsTheArgumentsValue()
        {
            int i = 10;
            Func<int, int> @delegate = PreDecrementAssign;
			var func = ExecuteLambda<Func<int, int>>(@delegate);
            int result = func(i);

            Assert.AreEqual(result, i - 1);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void PretDecrementAssignExpression_OfIntegerThatHasMinValueWithCheckedStatement_RaisesAnOverflowException()
        {
            Func<int> @delegate = PreDecrementMinValueWithChecked;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();
        }

        [TestMethod]
        public void PreDecrementAssignExpression_OfIntegerThatHasMinValue_CausesOverflowAndReturnsTheCorrectDecrementedNumber()
        {
            Func<int> @delegate = PreDecrementMinValueWithoutChecked;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();

            Assert.AreEqual(result, int.MaxValue);
        }

        private int PostDecrementAssignInReturnStatement(int i)
        {
            return i--;
        }

        private int PostDecrementAssign(int i)
        {
            i--;

            return i;
        }

        private int PostDecrementMinValueWithChecked()
        {
            int i = int.MinValue;

            checked
            {
                i--;
            }

            return i;
        }

        private int PostDecrementMinValueWithoutCheck()
        {
            int i = int.MinValue;

            i--;

            return i;
        }

        private int PreDecrementAssign(int i)
        {
            return --i;
        }

        private int PreDecrementMinValueWithChecked()
        {
            int i = int.MinValue;

            checked
            {
                --i;
            }

            return i;
        }

        private int PreDecrementMinValueWithoutChecked()
        {
            int i = int.MinValue;

            return --i;
        }
    }
}
