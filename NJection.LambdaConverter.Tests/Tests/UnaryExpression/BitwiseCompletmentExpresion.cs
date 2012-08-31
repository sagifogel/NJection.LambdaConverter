using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.UnaryExpression
{
    /// <summary>
    /// Summary description for OnesComplementExpression
    /// </summary>
    [TestClass]
    public class BitwiseCompletmentExpresion : BaseTest
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
        public void BitwiseCompletmentExpresion_OfIntGivenTheValueOfMaxValue_ReturnsMinValue()
        {
			Func<int> @delegate = OnesCompletment;
            var func = ExecuteLambda<Func<int>>(@delegate);
            var result = func();

            Assert.AreEqual(result, int.MinValue);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void BitwiseCompletmentExpresion_OfIntGivenTheValueOfMaxValueWithCheckedStatmenet__RaisesAnOverflowException()
        {
			Func<int> @delegate = OnesCompletmentWithCheckedStatement;
            var func = ExecuteLambda<Func<int>>(@delegate);
            var result = func();
        }

        private static int OnesCompletment()
        {
            int i = int.MaxValue;

            return ~i;
        }

        private static int OnesCompletmentWithCheckedStatement()
        {
            int result = 0;
            long i = long.MaxValue;

            checked
            {
                result = (int)~i;
            }

            return result;
        }
    }
}
