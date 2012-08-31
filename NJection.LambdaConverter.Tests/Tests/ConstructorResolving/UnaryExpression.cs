using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.ConstructorResolving
{
    [TestClass]
    public class UnaryExpression : BaseConstructorTest
    {
        private TestContext testContextInstance;

        public class DerivedClass : BaseClass
        {
            public static int I;
            public static uint U;
            public static double D;
            public static bool B;

            public DerivedClass() : base(I++) { }
            public DerivedClass(int i) : base(checked(I++)) { }
            public DerivedClass(uint u) : base(checked(U++)) { }
            public DerivedClass(char c) : base(U--) { }
            public DerivedClass(string u) : base(checked(I--)) { }
            public DerivedClass(byte b) : base(-(I)) { }
            public DerivedClass(sbyte b) : base(~I) { }
            public DerivedClass(bool b) : base(!B) { }
            //public DerivedClass(double d) : base(checked(D++)) { }
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
        #endregion

        [TestMethod]
        public void ConstructorResolving_UsingUnaryExpressionOfPostIncreamentForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            DerivedClass.I = int.MaxValue;
            var ctor = GetConstructorByType<DerivedClass>(Type.EmptyTypes);
            var func = ExecuteConstructorLambda<Func<DerivedClass>>(ctor);
            var derivedClass = func();
            int value = (int)derivedClass.BaseValue;

            Assert.AreEqual(value, int.MaxValue);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void ConstructorResolving_UsingUnaryExpressionOfCheckedPostIncreamentWithMaxValueOfIntForBaseConstructor_RaiseAnOverflowException() {
            DerivedClass.I = int.MaxValue;
            var ctor = GetConstructorByType<DerivedClass>(typeof(int));
            var func = ExecuteConstructorLambda<Func<int, DerivedClass>>(ctor);

            func(0);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void ConstructorResolving_UsingUnaryExpressionOfCheckedPostIncreamentOfUnsignedIntWithMaxValueForBaseConstructor_RaiseAnOverflowException() {
            DerivedClass.U = uint.MaxValue;
            var ctor = GetConstructorByType<DerivedClass>(typeof(uint));
            var func = ExecuteConstructorLambda<Func<uint, DerivedClass>>(ctor);

            func(0);
        }

        [TestMethod]
        public void ConstructorResolving_UsingUnaryExpressionOfPostDecreamentForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            DerivedClass.U = uint.MinValue;
            var ctor = GetConstructorByType<DerivedClass>(typeof(char));
            var func = ExecuteConstructorLambda<Func<char, DerivedClass>>(ctor);
            var derivedClass = func('A');
            uint value = (uint)derivedClass.BaseValue;

            Assert.AreEqual(value, 0u);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void ConstructorResolving_UsingUnaryExpressionOfCheckedPostDecreamentOfUnsignedIntWithValueOfZeroForBaseConstructor_RaiseAnOverflowException() {
            DerivedClass.I = int.MinValue;
            var ctor = GetConstructorByType<DerivedClass>(typeof(string));
            var func = ExecuteConstructorLambda<Func<string, DerivedClass>>(ctor);

            func(string.Empty);
        }

        [TestMethod]
        public void ConstructorResolving_UsingUnaryExpressionOfNegationForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            DerivedClass.I = 1000;
            var ctor = GetConstructorByType<DerivedClass>(typeof(byte));
            var func = ExecuteConstructorLambda<Func<byte, DerivedClass>>(ctor);
            var derivedClass = func(0);
            int value = (int)derivedClass.BaseValue;

            Assert.AreEqual(value, -1000);
        }

        [TestMethod]
        public void ConstructorResolving_UsingUnaryExpressionOfBitNotForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            DerivedClass.I = 0;
            var ctor = GetConstructorByType<DerivedClass>(typeof(sbyte));
            var func = ExecuteConstructorLambda<Func<sbyte, DerivedClass>>(ctor);
            var derivedClass = func(0);
            int value = (int)derivedClass.BaseValue;

            Assert.AreEqual(value, -1);
        }

        [TestMethod]
        public void ConstructorResolving_UsingUnaryExpressionOfNotForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(bool));
            var func = ExecuteConstructorLambda<Func<bool, DerivedClass>>(ctor);
            var derivedClass = func(false);
            bool value = (bool)derivedClass.BaseValue;

            Assert.IsTrue(value);
        }

        //[TestMethod] TODO: NRefactory Bug
        [ExpectedException(typeof(OverflowException))]
        public void Bugged_ConstructorResolving_UsingUnaryExpressionOfCheckedPostIncreamentForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            DerivedClass.I = int.MaxValue;
            var ctor = GetConstructorByType<DerivedClass>(typeof(double));
            var func = ExecuteConstructorLambda<Func<double, DerivedClass>>(ctor);

            func(0);
        }
    }
}
