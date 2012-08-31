using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.BinaryOperations
{
    /// <summary>
    /// Summary description for ArrayIndex
    /// </summary>
    [TestClass]
    public class ArrayIndex : BaseTest
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
        public void VectorArrayIndex_GivenSpecificIndex_ReturnsTheCorrelatedValueInTheVector()
        {            
			int ten = 10;
			Func<int, int> @delegate = GetArrayIndex;
            var func = ExecuteLambda<Func<int, int>>(@delegate);
            var result = func(ten);

            Assert.AreEqual(result, ten);
        }

        [TestMethod]
        public void MultiDimensionalArrayIndex_GivenSpecificIndexes_ReturnsTheCorrelatedValue()
        {
            Func<int, int, int> @delegate = GetMultiDimensionalArrayIndex;
			var func = ExecuteLambda<Func<int, int, int>>(@delegate);
            var result = func(1, 4);

            Assert.AreEqual(result, 10);
        }

        [TestMethod]
        public void JaggedMultiDimensionalArrayIndex_GivenSpecificIndexes_ReturnsTheCorrelatedValue()
        {
            Func<int, int, int, int> @delegate = GetJaggedMultiDimensionalJaggesArrayIndex;
			var func = ExecuteLambda<Func<int, int, int, int>>(@delegate);
            var result = func(1, 1, 3);

            Assert.AreEqual(result, 16);
        }

        private int GetArrayIndex(int index)
        {
            int[] ints = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            return ints[index];
        }

        private int GetMultiDimensionalArrayIndex(int i, int j)
        {
            int[,] ints = new[,] { { 1, 2, 3, 4, 5 }, { 6, 7, 8, 9, 10 } };

            return ints[i, j];
        }

        private int GetJaggedMultiDimensionalJaggesArrayIndex(int i, int j, int k)
        {
            var jagged = new int[2][,] { new int[,] { { 1, 2, 3, 4 }, { 5, 6, 7, 8 } }, new int[2, 4] { { 9, 10, 11, 12 }, { 13, 14, 15, 16 } } };

            return jagged[i][j, k];
        }
    }
}
