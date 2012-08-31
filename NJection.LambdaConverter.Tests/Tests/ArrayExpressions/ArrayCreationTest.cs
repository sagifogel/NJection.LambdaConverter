using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests
{
    [TestClass]
    public class ArrayCreationTest : BaseTest
    {
        #region Tests

        [TestMethod]
        public void VectorCreation_WithSpecifiedSizeOfAndInitializers_ReturnsTheCorrectUpperBoundAndValue()
        {
            Func<int[]> @delegate = VectorCreationWithSpecifiedSizeAndInitializers;
			var func = ExecuteLambda<Func<int[]>>(@delegate);
            int[] ints = func();
            int lastIndex = ints.GetUpperBound(0);

            Assert.IsTrue(lastIndex == 5 && ints[lastIndex] == 6);
        }

        [TestMethod]
        public void VectorCreation_WithoutSpecifiedSizeAndInitializers_ReturnsTheCorrectUpperBoundAndValue()
        {
            Func<int[]> @delegate = VectorCreationWithoutSpecifiedSizeAndInitializers;
			var func = ExecuteLambda<Func<int[]>>(@delegate);
            int[] ints = func();
            int lastIndex = ints.GetUpperBound(0);

            Assert.IsTrue(lastIndex == 5 && ints[lastIndex] == 6);
        }

        [TestMethod]
        public void VectorCreation_WithSpecifiedSizeAndWithoutInitializers_ReturnsTheCorrectUpperBound()
        {
            Func<int[]> @delegate = VectorWithSpecifiedSizeAndWithoutInitializers;
			var func = ExecuteLambda<Func<int[]>>(@delegate);
            int[] ints = func();
            int upperBoound = ints.GetUpperBound(0);

            Assert.IsTrue(upperBoound == 5);
        }

        [TestMethod]
        public void MultiDimensionalArrayCreation_WithSpecifiedSizeAndInitializers_ReturnsTheCorrectUpperBoundsAndValue()
        {
            Func<int[,]> @delegate = MultiDimensionalArrayWithSpecifiedSizeAndInitializers;
			var func = ExecuteLambda<Func<int[,]>>(@delegate);
            int[,] ints = func();
            int upperBoound = ints.GetUpperBound(0);
            int lowerBound = ints.GetLowerBound(0);

            Assert.IsTrue(upperBoound == 1 && lowerBound == 0 && ints[1, 2] == 6);
        }

        [TestMethod]
        public void MultiDimensionalArrayCreation_WithoutSpecifiedSizeAndInitializers_ReturnsTheCorrectUpperBoundsAndValue()
        {
            Func<int[,]> @delegate = MultiDimensionalArrayWithoutSpecifiedSizeAndInitializers;
			var func = ExecuteLambda<Func<int[,]>>(@delegate);
            int[,] ints = func();
            int upperBoound = ints.GetUpperBound(0);
            int lowerBound = ints.GetLowerBound(0);

            Assert.IsTrue(upperBoound == 1 && lowerBound == 0 && ints[1, 2] == 6);
        }

        [TestMethod]
        public void MultiDimensionalArrayCreation_WithSpecifiedSizeAndWithoutInitializers_ReturnsTheCorrectBounds()
        {
            Func<int[,]> @delegate = MultiDimensionalArrayWithSpecifiedSizeAndWithoutInitializers;
			var func = ExecuteLambda<Func<int[,]>>(@delegate);
            int[,] ints = func();
            int upperBoound = ints.GetUpperBound(0);
            int lowerBound = ints.GetLowerBound(0);

            Assert.IsTrue(upperBoound == 1 && lowerBound == 0);
        }

        [TestMethod]
        public void JaggedArrayCreation_WithSpecifiedSizeAndInitializers_ReturnsTheCorrectBoundsAndValue()
        {
            Func<int[][]> @delegate = JaggedArrayWithSpecifiedSizeAndInitializers;
			var func = ExecuteLambda<Func<int[][]>>(@delegate);
            int[][] ints = func();
            int upperBoound = ints.GetUpperBound(0);
            int lowerBound = ints.GetLowerBound(0);

            Assert.IsTrue(upperBoound == 1 && lowerBound == 0 && ints[1][3] == 8);
        }

        [TestMethod]
        public void JaggedArrayCreation_WithoutSpecifiedSizeAndInitializers_ReturnsTheCorrectBoundsAndValue()
        {
            Func<int[][]> @delegate = JaggedArrayWithoutSpecifiedSizeAndInitializers;
			var func = ExecuteLambda<Func<int[][]>>(@delegate);
            int[][] ints = func();
            int upperBoound = ints.GetUpperBound(0);
            int lowerBound = ints.GetLowerBound(0);

            Assert.IsTrue(upperBoound == 1 && lowerBound == 0 && ints[1][3] == 8);
        }

        [TestMethod]
        public void JaggedArrayCreation_WithSpecifiedSizeAndWithoutInitializers_ReturnsCorrectBounds()
        {
            Func<int[][]> @delegate = JaggedArrayWithSpecifiedSizeAndWithoutInitializers;
			var func = ExecuteLambda<Func<int[][]>>(@delegate);
            int[][] ints = func();
            int upperBoound = ints.GetUpperBound(0);
            int lowerBound = ints.GetLowerBound(0);

            Assert.IsTrue(upperBoound == 1 && lowerBound == 0);
        }

        [TestMethod]
        public void MultiDimensionalJaggedArray_WithSpecifiedSizeAndInitializers_ReturnsCorrectBoundsAndValue()
        {
            Func<int[][,]> @delegate = MultiDimensionalJaggedArrayWithSpecifiedSizeAndInitializers;
			var func = ExecuteLambda<Func<int[][,]>>(@delegate);
            int[][,] ints = func();
            int upperBoound = ints.GetUpperBound(0);
            int lowerBound = ints.GetLowerBound(0);

            Assert.IsTrue(upperBoound == 1 && lowerBound == 0 && ints[1][0, 3] == 8);
        }

        [TestMethod]
        public void MultiDimensionalJaggedArray_WithoutSpecifiedSizeAndInitializers_ReturnsCorrectBoundsAndValue()
        {
            Func<int[][,]> @delegate = MultiDimensionalJaggedArrayWithoutSpecifiedSizeAndInitializers;
			var func = ExecuteLambda<Func<int[][,]>>(@delegate);
            int[][,] ints = func();
            int upperBoound = ints.GetUpperBound(0);
            int lowerBound = ints.GetLowerBound(0);

            Assert.IsTrue(upperBoound == 1 && lowerBound == 0 && ints[1][0, 3] == 8);
        }

        [TestMethod]
        public void MultiDimensionalJaggedArray_WithSpecifiedSizeAndWithoutInitializers_ReturnsCorrectBounds()
        {
            Func<int[][,]> @delegate = MultiDimensionalJaggedArrayWithSpecifiedSizeAndWithoutInitializers;
			var func = ExecuteLambda<Func<int[][,]>>(@delegate);
            int[][,] ints = func();
            int upperBoound = ints.GetUpperBound(0);
            int lowerBound = ints.GetLowerBound(0);

            Assert.IsTrue(upperBoound == 1 && lowerBound == 0);
        }

        [TestMethod]
        public void WeckyArray_WithSpecifiedSizeAndWithoutInitializers_ReturnsCorrectBounds()
        {
            Func<int[,][][,]> @delegate = WeckyArrayWithSpecifiedSizeAndWithoutInitializers;
			var func = ExecuteLambda<Func<int[,][][,]>>(@delegate);
            int[,][][,] ints = func();
            int upperBoound = ints.GetUpperBound(0);
            int lowerBound = ints.GetLowerBound(0);

            Assert.IsTrue(upperBoound == 0 && lowerBound == 0);
        }

        #endregion Tests

        #region Methods

        public int[] VectorCreationWithSpecifiedSizeAndInitializers()
        {
            return new int[6] { 1, 2, 3, 4, 5, 6 };
        }

        public int[] VectorCreationWithoutSpecifiedSizeAndInitializers()
        {
            return new[] { 1, 2, 3, 4, 5, 6 };
        }

        public int[] VectorWithSpecifiedSizeAndWithoutInitializers()
        {
            return new int[6];
        }

        public int[,] MultiDimensionalArrayWithSpecifiedSizeAndInitializers()
        {
            return new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };
        }

        public int[,] MultiDimensionalArrayWithoutSpecifiedSizeAndInitializers()
        {
            return new int[,] { { 1, 2, 3 }, { 4, 5, 6 } };
        }

        public int[,] MultiDimensionalArrayWithSpecifiedSizeAndWithoutInitializers()
        {
            return new int[2, 3];
        }

        public int[][] JaggedArrayWithSpecifiedSizeAndInitializers()
        {
            return new int[2][] { new int[4] { 1, 2, 3, 4 }, new int[4] { 5, 6, 7, 8 } };
        }

        public int[][] JaggedArrayWithoutSpecifiedSizeAndInitializers()
        {
            return new int[][] { new int[4] { 1, 2, 3, 4 }, new int[4] { 5, 6, 7, 8 } };
        }

        public int[][] JaggedArrayWithSpecifiedSizeAndWithoutInitializers()
        {
            return new int[2][];
        }

        public int[][,] MultiDimensionalJaggedArrayWithSpecifiedSizeAndInitializers()
        {
            return new int[2][,] { new int[1, 4] { { 1, 2, 3, 4 } }, new int[1, 4] { { 5, 6, 7, 8 } } };
        }

        public int[][,] MultiDimensionalJaggedArrayWithoutSpecifiedSizeAndInitializers()
        {
            return new int[][,] { new int[,] { { 1, 2, 3, 4 } }, new int[,] { { 5, 6, 7, 8 } } };
        }

        public int[][,] MultiDimensionalJaggedArrayWithSpecifiedSizeAndWithoutInitializers()
        {
            return new int[2][,];
        }

        public int[,][][,] WeckyArrayWithSpecifiedSizeAndWithoutInitializers()
        {
            return new int[1, 1][][,];
        }

        #endregion Methods
    }
}
