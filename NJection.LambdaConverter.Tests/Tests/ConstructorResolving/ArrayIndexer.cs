using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.ConstructorResolving
{
    [TestClass]
    public class ArrayIndexer : BaseConstructorTest
    {
        private TestContext testContextInstance;

        private static int[] _vector = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        private static int[,] _matrix = new int[,] { { 1, 2, 3, 4 }, { 5, 6, 7, 8 } };
        private static int[][] _jaggedArray = new int[][] { new[] { 1, 2, 3, 4 }, new int[] { 5, 6, 7, 8 } };
        private static int[][][] _multipulejaggedArray = new int[][][] { new int[][] { new[] { 1, 2, 3, 4 }, new int[] { 5, 6, 7, 8 } } };
        private static int[,][][,] _weckyArray = new int[,][][,] { { new int[][,] { new[,] { { 1, 2 } } } }, { new int[][,] { new[,] { { 3, 4 } } } } };

        public class DerivedClass : BaseClass
        {
            public DerivedClass()
                : base(_vector[7]) {
            }

            public DerivedClass(int i)
                : base(_matrix[1, 3]) {
            }

            public DerivedClass(double i)
                : base(_jaggedArray[1][3]) {
            }

            public DerivedClass(string s)
                : base(_jaggedArray[1][3]) {
            }

            public DerivedClass(bool i)
                : base(_weckyArray[1, 0][0][0, 1]) {
            }
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
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

        #endregion Additional test attributes

        [TestMethod]
        public void ConstructorResolving_UsingVectorIndexingForBaseConstructor_ReturnsLastElementInVector() {
            var ctor = GetConstructorByType<DerivedClass>(Type.EmptyTypes);
            var func = ExecuteConstructorLambda<Func<DerivedClass>>(ctor);
            var derivedClass = func();

            Assert.AreEqual(derivedClass.BaseValue, _vector.Last());
        }

        [TestMethod]
        public void ConstructorResolving_UsingMatrixIndexingForBaseConstructor_ReturnsLastElementInMatrix() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(int));
            var func = ExecuteConstructorLambda<Func<int, DerivedClass>>(ctor);
            var derivedClass = func(10);

            Assert.AreEqual(derivedClass.BaseValue, _matrix[1, 3]);
        }

        [TestMethod]
        public void ConstructorResolving_UsingJaggedArayIndexingForBaseConstructor_ReturnsLastElementInJaggedArray() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(double));
            var func = ExecuteConstructorLambda<Func<double, DerivedClass>>(ctor);
            var derivedClass = func(1);

            Assert.AreEqual(derivedClass.BaseValue, _jaggedArray[1][3]);
        }

        [TestMethod]
        public void ConstructorResolving_UsingMultipuleJaggedArayIndexingForBaseConstructor_ReturnsLastElementInJaggedArray() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(string));
            var func = ExecuteConstructorLambda<Func<string, DerivedClass>>(ctor);
            var derivedClass = func(string.Empty);

            Assert.AreEqual(derivedClass.BaseValue, _jaggedArray[1][3]);
        }

        [TestMethod]
        public void ConstructorResolving_UsingWeckyArrayIndexingForBaseConstructor_ReturnsLastElementInArray() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(bool));
            var func = ExecuteConstructorLambda<Func<bool, DerivedClass>>(ctor);
            var derivedClass = func(true);

            Assert.AreEqual(derivedClass.BaseValue, _weckyArray[1, 0][0][0, 1]);
        }
    }
}