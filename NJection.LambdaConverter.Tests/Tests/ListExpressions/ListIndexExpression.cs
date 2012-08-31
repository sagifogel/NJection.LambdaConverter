using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests
{
    /// <summary>
    /// Summary description for ListCreation
    /// </summary>
    [TestClass]
    public class ListIndexExpression : BaseTest
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
        public void ListIndex_GivenASpecificIndex_ReturnsTheCorrelatedValue()
        {
            Func<int, int> @delegate = GetListIndex;
			var func = ExecuteLambda<Func<int, int>>(@delegate);
            var result = func(10);

            Assert.AreEqual(result, 10);
        }

        [TestMethod]
        public void ListIndex_OfInerListInANestedListStructure_ReturnsTheCorrelatedValue()
        {
            Func<int, int> @delegate = GetNestedListIndex;
			var func = ExecuteLambda<Func<int, int>>(@delegate);
            var result = func(10);

            Assert.AreEqual(result, 10);
        }

        private int GetListIndex(int index)
        {
            var ints = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            return ints[index];
        }

        private int GetNestedListIndex(int i)
        {
            var ints = new List<List<int>> { new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } };

            return ints[0][i];
        }
    }
}
