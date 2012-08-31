using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.UnaryExpression
{
    /// <summary>
    /// Summary description for UnboxExpression
    /// </summary>
    [TestClass]
    public class UnboxExpression : BaseTest
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
        public void UnboxExpression_GivenSpecificValueOfIntToBox_ReturnsTheUnboxedOriginalValue()
        {   
            int toBeBoxed = 10;
            Func<int, int> @delegate = BoxUnbox;
			var func = ExecuteLambda<Func<int, int>>(@delegate);
            var result = func(toBeBoxed);

            Assert.AreEqual(result, toBeBoxed);
        }

        private static int BoxUnbox(int toBeBoxed)
        {
            object boxed = toBeBoxed;

            return (int)boxed;
        }
    }
}
