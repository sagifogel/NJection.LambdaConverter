using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests
{
    /// <summary>
    /// Summary description for GotoExpression
    /// </summary>
    [TestClass]
    public class GotoExpression : BaseTest
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
        public void GoToExpression_GivenTwoDifferentInputs_ReturnsTwoDifferentValuesUsingDifferentScopesThroughBranching()
        {
			Func<int, int> @delegate = Branching;
			var func = ExecuteLambda<Func<int, int>>(@delegate);
            var first = func(100);
            var second = func(1000);

            Assert.AreNotEqual(first, second);
        }

        private int Branching(int input)
        {
            int x = 200, y = 4;
            int count = 0;
            string[,] myArray = new string[x, y];
            string message = string.Empty;
            string myNumber = input.ToString();

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    myArray[i, j] = (++count).ToString();
                }
            }

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    if (myArray[i, j].Equals(myNumber))
                    {
                        goto Found;
                    }
                }
            }

            message = string.Format("The number {0} was not found.", myNumber);
            goto Finish;

        Found:
            {
                message = string.Format("The number {0} is found.", myNumber);
                x = x + 10;
            }
        Finish:

            myArray = new string[x, y];
            return x;
        }
    }
}
