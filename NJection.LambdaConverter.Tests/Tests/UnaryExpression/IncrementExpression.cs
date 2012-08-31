using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.UnaryExpression
{
    /// <summary>
    /// Summary description for IncrementExpression
    /// </summary>
    [TestClass]
    public class IncrementExpression : BaseTest
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
        public void PostIncrementAssignExpression_OfIntegerAsAnArgumentAtTheReturnStatement_DoesNotChangeTheArgumentsOriginalValue()
        {
            int i = 10;
			Func<int, int> @delegate = PostIncrementAssignInReturnStatement;
			var func = ExecuteLambda<Func<int, int>>(@delegate);
            int result = func(i);

            Assert.AreEqual(result, i);
        }

        [TestMethod]
        public void PostIncrementAssignExpression_OfIntegerBeforeTheReturnStatement_IncrementsTheArgumentsValue()
        {
            int i = 10;
			Func<int, int> @delegate = PostIncrementAssign;
			var func = ExecuteLambda<Func<int, int>>(@delegate);
            int result = func(i);

            Assert.AreEqual(result, i + 1);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void PostIncrementAssignExpression_OfIntegerThatHasMaxValueWithCheckedStatement_RaisesAnOverflowException()
        {
			Func<int> @delegate = PostIncrementMaxValueWithChecked;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();
        }

        [TestMethod]
        public void PostIncrementAssignExpression_OfIntegerThatHasMaxValue_CausesOverflowAndReturnsTheCorrectIncrementedNumber()
        {
			Func<int> @delegate = PostIncrementMaxValueWithoutCheck;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();

            Assert.AreEqual(result, int.MinValue);
        }

        [TestMethod]
        public void PreIncrementAssignExpression_OfInteger_IncrementsTheArgumentsValue()
        {
            int i = 10;
			Func<int, int> @delegate = PreIncrementAssign;
			var func = ExecuteLambda<Func<int, int>>(@delegate);
            int result = func(i);

            Assert.AreEqual(result, i + 1);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void PretIncrementAssignExpression_OfIntegerThatHasMaxValueWithCheckedStatement_RaisesAnOverflowException()
        {
			Func<int> @delegate = PreIncrementMaxValueWithChecked;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();
        }

        [TestMethod]
        public void PreIncrementAssignExpression_OfIntegerThatHasMaxValue_CausesOverflowAndReturnsTheCorrectIncrementedNumber()
        {
			Func<int> @delegate = PreIncrementMaxValueWithoutChecked;
			var func = ExecuteLambda<Func<int>>(@delegate);
            int result = func();

            Assert.AreEqual(result, int.MinValue);
        }

        private int PostIncrementAssignInReturnStatement(int i)
        {
            return i++;
        }

        private int PostIncrementAssign(int i)
        {
            i++;

            return i;
        }

        private int PostIncrementMaxValueWithChecked()
        {
            int i = int.MaxValue;

            checked
            {
                i++;
            }

            return i;
        }

        private int PostIncrementMaxValueWithoutCheck()
        {
            int i = int.MaxValue;

            i++;

            return i;
        }

        private int PreIncrementAssign(int i)
        {
            return ++i;
        }

        private int PreIncrementMaxValueWithChecked()
        {
            int i = int.MaxValue;

            checked
            {
                ++i;
            }

            return i;
        }

        private int PreIncrementMaxValueWithoutChecked()
        {
            int i = int.MaxValue;

            return ++i;
        }
    }
}
