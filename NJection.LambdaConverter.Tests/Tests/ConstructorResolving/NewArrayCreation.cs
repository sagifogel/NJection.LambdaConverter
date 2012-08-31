using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.ConstructorResolving
{
    [TestClass]
    public class NewArrayCreation : BaseConstructorTest
    {
        private TestContext testContextInstance;

        public enum Month
        {
            Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec
        }

        public class DerivedClass : BaseClass
        {
            public int Value { get; set; }

            public DerivedClass()
                : base(new int[] { 1, 2, 3, 4, 5, 6 }) {
                Value = 0;
            }

            public DerivedClass(char c)
                : base(new char[6] { '1', '2', '3', '4', '5', '6' }) {
                Value = 0;
            }

            public DerivedClass(bool b)
                : base(new bool[] { true, false, true }) {
            }

            public DerivedClass(double d)
                : base(new double[,] { { 1, 2 }, { 3, 4 } }) {
            }

            //public DerivedClass(decimal d)
            //    : base(new decimal[2, 2] { { 1, 2 }, { 3, 4 } })
            //{
            //}

            public DerivedClass(decimal d)
                : base(new decimal[] { decimal.MaxValue, decimal.One, decimal.MinValue, decimal.MinValue }) {
                Value = 0;
            }

            public DerivedClass(short s)
                : base(new short[][,] { new short[,] { { 1, 2, 3, 4 } }, new short[,] { { 5, 6, 7, 8 } } }) {
            }

            public DerivedClass(uint u)
                : base(new uint[2][,] { new uint[1, 4] { { 1, 2, 3, 4 } }, new uint[1, 4] { { 5, 6, 7, 8 } } }) {
            }

            public DerivedClass(sbyte s)
                : base(new sbyte[1, 1][][,]) {
            }

            public DerivedClass(Month m)
                : base(new Month[] { Month.Mar, Month.Jun }) {
            }

            public DerivedClass(DateTime d)
                : base(new DateTime[] { DateTime.Now, DateTime.Today, DateTime.UtcNow, DateTime.MinValue }) {
            }

            public DerivedClass(object obj)
                : base(new object[] { DateTime.Now, 10, true, new int[] { 10 } }) {
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
        public void ConstructorResolving_UsingNewVectorOfIntegersWithoutSpecifiedSizeAndIntializerForBaseConstructor_ReturnsProxiedInstanceWithVector() {
            var ctor = GetConstructorByType<DerivedClass>(Type.EmptyTypes);
            var func = ExecuteConstructorLambda<Func<DerivedClass>>(ctor);
            var derivedClass = func();
            var proxiedType = derivedClass;

            Assert.IsTrue(proxiedType.BaseValue.GetType().IsArray);
            Assert.IsTrue((proxiedType.BaseValue as int[]).SequenceEqual(new[] { 1, 2, 3, 4, 5, 6 }));
        }

        [TestMethod]
        public void ConstructorResolving_UsingNewVectorOfCharsWithSpecifiedSizeAndIntializerIntegersForBaseConstructor_ReturnsProxiedInstanceWithVector() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(char));
            var func = ExecuteConstructorLambda<Func<char, DerivedClass>>(ctor);
            var derivedClass = func('1');
            var proxiedType = derivedClass;

            Assert.IsTrue(proxiedType.BaseValue.GetType().IsArray);
            Assert.IsTrue((proxiedType.BaseValue as char[]).SequenceEqual(new char[] { '1', '2', '3', '4', '5', '6' }));
        }

        [TestMethod]
        public void ConstructorResolving_UsingNewVectorOfBooleansWithoutSpecifiedSizeAndInitializerForBaseConstructor_ReturnsProxiedInstanceWithVector() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(bool));
            var func = ExecuteConstructorLambda<Func<bool, DerivedClass>>(ctor);
            var derivedClass = func(true);
            var proxiedType = derivedClass;

            Assert.IsTrue(proxiedType.BaseValue.GetType().IsArray);
            Assert.IsTrue((proxiedType.BaseValue as bool[]).SequenceEqual(new[] { true, false, true }));
        }

        [TestMethod]
        public void ConstructorResolving_UsingNewVectorOfEnumsWithoutSpecifiedSizeAndInitializerForBaseConstructor_ReturnsProxiedInstanceWithVector() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(Month));
            var func = ExecuteConstructorLambda<Func<Month, DerivedClass>>(ctor);
            var derivedClass = func(default(Month));
            var proxiedType = derivedClass;

            Assert.IsTrue(proxiedType.BaseValue.GetType().IsArray);
        }

        [TestMethod]
        public void ConstructorResolving_UsingNewVectorOfDateTimeWithoutSpecifiedSizeAndInitializerForBaseConstructor_ReturnsProxiedInstanceWithVector() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(DateTime));
            var func = ExecuteConstructorLambda<Func<DateTime, DerivedClass>>(ctor);
            var derivedClass = func(default(DateTime));
            var proxiedType = derivedClass;

            Assert.IsTrue(proxiedType.BaseValue.GetType().IsArray);
        }

        [TestMethod]
        public void ConstructorResolving_UsingNewVectorOfDecimalsWithoutSpecifiedSizeAndInitializerForBaseConstructor_ReturnsProxiedInstanceWithVector() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(decimal));
            var func = ExecuteConstructorLambda<Func<decimal, DerivedClass>>(ctor);
            var derivedClass = func(1);
            var proxiedType = derivedClass;

            Assert.IsTrue(proxiedType.BaseValue.GetType().IsArray);
            Assert.IsTrue((proxiedType.BaseValue as decimal[]).SequenceEqual(new[] { decimal.MaxValue, decimal.One, decimal.MinValue, decimal.MinValue }));
        }

        [TestMethod]
        public void ConstructorResolving_UsingNewVectorOfObjectsWithoutSpecifiedSizeAndInitializersHavingSomeArgumentsThatNeedCastingForBaseConstructor_ReturnsProxiedInstanceWithVector() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(object));
            var func = ExecuteConstructorLambda<Func<object, DerivedClass>>(ctor);
            var derivedClass = func(null);
            var proxiedType = derivedClass;
            var array = proxiedType.BaseValue as object[];

            Assert.IsTrue(proxiedType.BaseValue.GetType().IsArray);
            Assert.IsInstanceOfType(array[0], typeof(DateTime));
            Assert.AreEqual(array[1], 10);
            Assert.AreEqual(array[2], true);
            Assert.IsTrue(array[3].GetType().IsArray);
            Assert.IsTrue((array[3] as int[]).SequenceEqual(new[] { 10 }));
        }

        [TestMethod]
        public void ConstructorResolving_UsingNewMultiDimensionalArrayOfDoublesWithoutSpecifiedSizeAndIntializersForBaseConstructor_ReturnsProxiedInstanceWithMultiDimensionalArray() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(double));
            var func = ExecuteConstructorLambda<Func<double, DerivedClass>>(ctor);
            var derivedClass = func(1);
            var proxiedType = derivedClass;

            Assert.IsTrue(proxiedType.BaseValue.GetType().IsArray);
        }

        //[TestMethod] TODO: NRefactory Bug
        public void ConstructorResolving_UsingNewMultiDimensionalArrayOfDecimalsWithSpecifiedSizeAndIntializersForBaseConstructor_ReturnsProxiedInstanceWithMultiDimensionalArray() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(decimal));
            var func = ExecuteConstructorLambda<Func<decimal, DerivedClass>>(ctor);
            var derivedClass = func(1);
            var proxiedType = derivedClass;

            Assert.IsTrue(proxiedType.BaseValue.GetType().IsArray);
        }

        [TestMethod]
        public void ConstructorResolving_UsingNewVectorOfJaggedArrayWithSpecifiedSizeAndInitializersForBaseConstructor_ReturnsProxiedInstanceWithNewVectorOfJaggedArray() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(uint));
            var func = ExecuteConstructorLambda<Func<uint, DerivedClass>>(ctor);
            var derivedClass = func(1);
            var proxiedType = derivedClass;

            Assert.IsTrue(proxiedType.BaseValue.GetType().IsArray);
        }

        [TestMethod]
        public void ConstructorResolving_UsingNewVectorOfJaggedArrayWithoutSpecifiedSizeAndInitializersForBaseConstructor_ReturnsProxiedInstanceWithNewVectorOfJaggedArray() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(short));
            var func = ExecuteConstructorLambda<Func<short, DerivedClass>>(ctor);
            var derivedClass = func(1);
            var proxiedType = derivedClass;

            Assert.IsTrue(proxiedType.BaseValue.GetType().IsArray);
        }

        [TestMethod]
        public void ConstructorResolving_UsingNewMultiDimensionalJaggedArrayWithSpecifiedSizeAndInitializersForBaseConstructor_ReturnsProxiedInstanceWithMultiDimensionalJaggedArray() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(uint));
            var func = ExecuteConstructorLambda<Func<uint, DerivedClass>>(ctor);
            var derivedClass = func(1);
            var proxiedType = derivedClass;

            Assert.IsTrue(proxiedType.BaseValue.GetType().IsArray);
        }

        [TestMethod]
        public void ConstructorResolving_UsingWeckyArrayWithSpecifiedSizeAndInitializersForBaseConstructor_ReturnsProxiedInstanceWithWeckyArray() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(sbyte));
            var func = ExecuteConstructorLambda<Func<sbyte, DerivedClass>>(ctor);
            var derivedClass = func(1);
            var proxiedType = derivedClass;

            Assert.IsTrue(proxiedType.BaseValue.GetType().IsArray);
        }
    }
}