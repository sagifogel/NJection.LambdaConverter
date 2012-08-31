using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.BinaryOperations
{
    /// <summary>
    /// Summary description for PowerExpression
    /// </summary>
    [TestClass]
    public class PowerExpression : BaseTest
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
        public void PowerExpression_GivenASpecificValue_ReturnsValueMultipliedByTwo()
        {
            var argument = 5;
			Func<int, int> @delegate = Power;
			var func = ExecuteLambda<Func<int, int>>(@delegate);
            var result = func(argument);

            Assert.AreEqual(result, Math.Pow(argument, 2));
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void PowerAssignExpression_GivenSpecificValueOfMaxValue_CausesAndOverflowAndRaisesAnException()
        {
            var argument = int.MaxValue;
			Func<int, int> @delegate = PowerChecked;
			var func = ExecuteLambda<Func<int, int>>(@delegate);
            var result = func(argument);
        }

        [TestMethod]
        public void PowerAssignExpression_GivenSpecificValueOfMaxValue_CausesOverflowAndReturnsTheOverflowValue()
        {
            var argument = int.MaxValue;
			Func<int, int> @delegate = PowerUnchecked;
			var func = ExecuteLambda<Func<int, int>>(@delegate);
            var result = func(argument);

            Assert.AreEqual(result, int.MinValue);
        }

        private int Power(int i)
        {
            i = (int)Math.Pow(i ,2);

            return (int)i;
        }

        private int PowerChecked(int i)
        {
            checked
            {
                i = (int)Math.Pow(i, 2);
            }

            return i;
        }

        private int PowerUnchecked(int i)
        {
            unchecked
            {
                i = (int)Math.Pow(i, 2);
            }

            return i;
        }
    }
}
