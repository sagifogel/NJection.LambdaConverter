using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests
{
    /// <summary>
    /// Summary description for Loops
    /// </summary>
    [TestClass]
    public class Loops : BaseTest
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
        public void ForEachLoop_CreatedByIteratingOverAList_ReturnsTheLastElementOfTheList()
        {
			Func<string> @delegate = ForEachLoopReturnsLastElement;
			var func = ExecuteLambda<Func<string>>(@delegate);
            var lastElement = func();

            Assert.AreEqual(lastElement, "%");
        }

        [TestMethod]
        public void ForLoop_CreatedByIteratingOverAList_ReturnsTheLastElementOfTheList()
        {
			Func<string> @delegate = ForLoopReturnsLastElement;
			var func = ExecuteLambda<Func<string>>(@delegate);
            var lastElement = func();

            Assert.AreEqual(lastElement, "%");
        }

        [TestMethod]
        public void WhileLoop_CreatedByIteratingOverAList_ReturnsTheLastElementOfTheList()
        {
			Func<string> @delegate = WhileLoopReturnsLastElement;
			var func = ExecuteLambda<Func<string>>(@delegate);
            var lastElement = func();

            Assert.AreEqual(lastElement, "%");
        }

        [TestMethod]
        public void DoWhile_CreatedByIteratingOverAList_ReturnsTheLastElementOfTheList()
        {
			Func<string> @delegate = DoWhileLoopReturnsLastElement;
			var func = ExecuteLambda<Func<string>>(@delegate);
            var lastElement = func();

            Assert.AreEqual(lastElement, "%");
        }

        [TestMethod]
        public void NestedForLoops_IteratingOverMultiDimensionalArray_ReturnsTheLastElementOfTheArray()
        {
			Func<int> @delegate = NestedForLoopsReturnsLastElement;
			var func = ExecuteLambda<Func<int>>(@delegate);
            var lastElement = func();

            Assert.AreEqual(lastElement, 10);
        }

        [TestMethod]
        public void NestedForEachLoops_IteratingOverNestedLists_ReturnsTheLastElementOfTheLastList()
        {
			Func<int> @delegate = NestedForEachLoopsReturnsLastElement;
			var func = ExecuteLambda<Func<int>>(@delegate);
            var lastElement = func();

            Assert.AreEqual(lastElement, 10);
        }

        [TestMethod]
        public void ForLoop_IteratingOverListOfintsWithContinueStatmentOnOddElements_ReturnsTheLastOddNumberElement()
        {
			Func<int> @delegate = ForLoopWithContinueStatementReturnsLastElement;
			var func = ExecuteLambda<Func<int>>(@delegate);
            var lastElement = func();

            Assert.AreEqual(lastElement, 10);
        }

        [TestMethod]
        public void ForLoop_IteratingOverListOfintsWithBreakStatment_ReturnsTheLastElementBeforeTheBreak()
        {
			Func<int> @delegate = ForLoopWithBreakStatementReturnsLastElementBeforeBreak;
			var func = ExecuteLambda<Func<int>>(@delegate);
            var lastElement = func();

            Assert.AreEqual(lastElement, 5);
        }
        
        private string ForEachLoopReturnsLastElement()
        {
            string element = string.Empty;
            List<string> strings = new List<string> { "!", "@", "#", "$", "%" };

            foreach (var item in strings)
            {
                element = item;
            }

            return element;
        }

        private string ForLoopReturnsLastElement()
        {
            string element = string.Empty;
            List<string> strings = new List<string> { "!", "@", "#", "$", "%" };

            for (int i = 0; i < strings.Count; i++)
            {
                element = strings[i];
            }

            return element;
        }

        private string WhileLoopReturnsLastElement()
        {
            int i = 0;
            string element = string.Empty;
            List<string> strings = new List<string> { "!", "@", "#", "$", "%" };

            while (i < strings.Count)
            {
                element = strings[i];
                i++;
            }

            return element;
        }

        private string DoWhileLoopReturnsLastElement()
        {
            int i = 0;
            string element = string.Empty;
            List<string> strings = new List<string> { "!", "@", "#", "$", "%" };

            do
            {
                element = strings[i];
                i++;
            }
            while (i < strings.Count);

            return element;
        }

        private int NestedForLoopsReturnsLastElement()
        {
            int element = 0;
            int[,] ints = new int[,] { { 1, 2, 3, 4, 5 }, { 6, 7, 8, 9, 10 } };

            for (int i = 0; i < ints.GetUpperBound(0) + 1; i++)
            {
                for (int j = 0; j < ints.GetUpperBound(1) + 1; j++)
                {
                    element = ints[i, j];
                }
            }

            return element;
        }

        private int NestedForEachLoopsReturnsLastElement()
        {
            int element = 0;
            List<List<int>> stringList = new List<List<int>>();

            stringList.Add(new List<int> { 1, 2, 3, 4, 5 });
            stringList.Add(new List<int> { 6, 7, 8, 9, 10 });

            foreach (var list in stringList)
            {
                foreach (var item in list)
                {
                    element = item;
                }
            }

            return element;
        }

        private int ForLoopWithContinueStatementReturnsLastElement()
        {
            int element = 0;
            List<int> ints = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            for (int i = 0; i < ints.Count; i++)
            {
                int value = ints[i];

                if (value % 2 != 0)
                {
                    continue;
                }

                element = value;
            }

            return element;
        }

        private int ForLoopWithBreakStatementReturnsLastElementBeforeBreak()
        {
            int element = 0;
            List<int> ints = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            for (int i = 0; i < ints.Count; i++)
            {
                int value = ints[i];

                if (value == 6)
                {
                    break;
                }

                element = value;
            }

            return element;
        }
    }
}
