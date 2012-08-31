using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.ConstructorResolving
{
    [TestClass]
    public class BinaryExpression : BaseConstructorTest
    {
        public class DerivedClass : BaseClass
        {
            public static int Value1 { get; set; }
            public static int Value2 { get; set; }
            public static bool Value3 { get; set; }
            public static bool Value4 { get; set; }
            public static object Value5 { get; set; }
            public static uint Value6 { get; set; }

            public int Value { get; set; }

            static DerivedClass() {
                Value1 = 100;
            }

            public DerivedClass() : base(Value1 == 100) { Value = 0; }
            public DerivedClass(int i) : base(Value1 != 100) { Value = 0; }
            public DerivedClass(double d) : base(Value1 > 100) { Value = 0; }
            public DerivedClass(string s) : base(Value1 >= 100) { Value = 0; }
            public DerivedClass(short s) : base(Value6 >= Value1) { Value = 0; }
            public DerivedClass(bool b) : base(Value1 < 100) { Value = 0; }
            public DerivedClass(char c) : base(Value1 <= 100) { Value = 0; }
            public DerivedClass(byte b) : base(++Value1) { Value = 0; }
            public DerivedClass(object o) : base(--Value1) { Value = 0; }
            public DerivedClass(ushort i) : base(Value1 & Value2) { Value = 0; }
            public DerivedClass(ulong i) : base(Value1 | Value2) { Value = 0; }
            public DerivedClass(DateTime i) : base(Value3 && Value4) { Value = 0; }
            public DerivedClass(DayOfWeek i) : base(Value3 || Value4) { Value = 0; }
            public DerivedClass(sbyte i) : base(Value1 >> Value2) { Value = 0; }
            public DerivedClass(decimal o) : base(Value1 << Value2) { Value = 0; }
            public DerivedClass(long i) : base(Value5 ?? new object()) { Value = 0; }
            public DerivedClass(int i, int i2) : base(Value1 ^ Value2) { Value = 0; }
            public DerivedClass(double d, double d2) : base(Value1 + Value2) { Value = 0; }
            public DerivedClass(string s, string s2) : base(Value1 - Value2) { Value = 0; }
            public DerivedClass(bool b, bool b2) : base(Value1 * Value2) { Value = 0; }
            public DerivedClass(char c, char c2) : base(Value1 / Value2) { Value = 0; }
            public DerivedClass(byte b, byte b2) : base(Value1 % Value2) { Value = 0; }
            public DerivedClass(UIntPtr ptr) : base(checked(Value1 + uint.MaxValue)) { Value = 0; } 
        }

        private TestContext testContextInstance;

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

        [TestInitialize()]
        public void Cleanup() {
            Initialize();
        }

        public void Initialize() {
            DerivedClass.Value1 = 100;
            DerivedClass.Value2 = 4;
            DerivedClass.Value3 = true;
            DerivedClass.Value4 = false;
            DerivedClass.Value6 = 200;
        }

        #endregion Additional test attributes

        [TestMethod]
        public void ConstructorResolving_UsingBinaryExpressionOfEqualityForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(Type.EmptyTypes);
            var func = ExecuteConstructorLambda<Func<DerivedClass>>(ctor);
            var derivedClass = func();
            bool value = (bool)derivedClass.BaseValue;

            Assert.IsTrue(value);
        }

        [TestMethod]
        public void ConstructorResolving_UsingBinaryExpressionOfInEqualityForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(int));
            var func = ExecuteConstructorLambda<Func<int, DerivedClass>>(ctor);
            var derivedClass = func(0);
            bool value = (bool)derivedClass.BaseValue;

            Assert.IsFalse(value);
        }

        [TestMethod]
        public void ConstructorResolving_UsingBinaryExpressionOfGreaterThanForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(double));
            var func = ExecuteConstructorLambda<Func<double, DerivedClass>>(ctor);
            var derivedClass = func(0);
            bool value = (bool)derivedClass.BaseValue;

            Assert.IsFalse(value);
        }

        [TestMethod]
        public void ConstructorResolving_UsingBinaryExpressionOfGreaterThanOrEqualsForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(string));
            var func = ExecuteConstructorLambda<Func<string, DerivedClass>>(ctor);
            var derivedClass = func(string.Empty);
            bool value = (bool)derivedClass.BaseValue;

            Assert.IsTrue(value);
        }

        [TestMethod]
        public void ConstructorResolving_UsingBinaryExpressionOfUnsignedGreaterThanOrEqualsForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(short));
            var func = ExecuteConstructorLambda<Func<short, DerivedClass>>(ctor);
            var derivedClass = func(0);
            bool value = (bool)derivedClass.BaseValue;

            Assert.IsTrue(value);
        }

        [TestMethod]
        public void ConstructorResolving_UsingBinaryExpressionOfLowerThanForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(bool));
            var func = ExecuteConstructorLambda<Func<bool, DerivedClass>>(ctor);
            var derivedClass = func(false);
            bool value = (bool)derivedClass.BaseValue;

            Assert.IsFalse(value);
        }

        [TestMethod]
        public void ConstructorResolving_UsingBinaryExpressionOfLowerThanOrEqualsForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(char));
            var func = ExecuteConstructorLambda<Func<char, DerivedClass>>(ctor);
            var derivedClass = func('A');
            bool value = (bool)derivedClass.BaseValue;

            Assert.IsTrue(value);
        }

        [TestMethod]
        public void ConstructorResolving_UsingBinaryExpressionOfPreIncreamentForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(byte));
            var func = ExecuteConstructorLambda<Func<byte, DerivedClass>>(ctor);
            var derivedClass = func(0);
            int value = (int)derivedClass.BaseValue;

            Assert.AreEqual(value, 101);
        }

        [TestMethod]
        public void ConstructorResolving_UsingBinaryExpressionOfPreDecreamentForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(object));
            var func = ExecuteConstructorLambda<Func<object, DerivedClass>>(ctor);
            var derivedClass = func(null);
            int value = (int)derivedClass.BaseValue;

            Assert.AreEqual(value, 99);
        }
        
        [TestMethod]
        public void ConstructorResolving_UsingBinaryExpressionOfBitwiseAndForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(UInt16));
            var func = ExecuteConstructorLambda<Func<UInt16, DerivedClass>>(ctor);
            var derivedClass = func(0);
            int value = (int)derivedClass.BaseValue;

            Assert.AreEqual(value, 4);
        }

        [TestMethod]
        public void ConstructorResolving_UsingBinaryExpressionOfBitwiseOrForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(UInt64));
            var func = ExecuteConstructorLambda<Func<UInt64, DerivedClass>>(ctor);
            var derivedClass = func(0);
            int value = (int)derivedClass.BaseValue;

            Assert.AreEqual(value, 100);
        }

        [TestMethod]
        public void ConstructorResolving_UsingBinaryExpressionOfConditionalAndForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(DateTime));
            var func = ExecuteConstructorLambda<Func<DateTime, DerivedClass>>(ctor);
            var derivedClass = func(DateTime.Now);
            bool value = (bool)derivedClass.BaseValue;

            Assert.IsFalse(value);
        }

        [TestMethod]
        public void ConstructorResolving_UsingBinaryExpressionOfConditionalOrForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(DayOfWeek));
            var func = ExecuteConstructorLambda<Func<DayOfWeek, DerivedClass>>(ctor);
            var derivedClass = func(0);
            bool value = (bool)derivedClass.BaseValue;

            Assert.IsTrue(value);
        }

        [TestMethod]
        public void ConstructorResolving_UsingBinaryExpressionOfShiftRightForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(sbyte));
            var func = ExecuteConstructorLambda<Func<sbyte, DerivedClass>>(ctor);
            var derivedClass = func(0);
            int value = (int)derivedClass.BaseValue;

            Assert.AreEqual(value, 6);
        }

        [TestMethod]
        public void ConstructorResolving_UsingBinaryExpressionOfShiftLeftForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(decimal));
            var func = ExecuteConstructorLambda<Func<decimal, DerivedClass>>(ctor);
            var derivedClass = func(0);
            int value = (int)derivedClass.BaseValue;

            Assert.AreEqual(value, 1600);
        }

        [TestMethod]
        public void ConstructorResolving_UsingBinaryExpressionOfNullCoalescingForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(long));
            var func = ExecuteConstructorLambda<Func<long, DerivedClass>>(ctor);
            var derivedClass = func(0);

            Assert.IsNotNull(derivedClass.BaseValue);
        }

        [TestMethod]
        public void ConstructorResolving_UsingBinaryExpressionOfXorForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(int), typeof(int));
            var func = ExecuteConstructorLambda<Func<int, int, DerivedClass>>(ctor);
            var derivedClass = func(0, 0);
            int value = (int)derivedClass.BaseValue;

            Assert.AreEqual(value, 96);
        }

        [TestMethod]
        public void ConstructorResolving_UsingBinaryExpressionOfAdditonForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(double), typeof(double));
            var func = ExecuteConstructorLambda<Func<double, double, DerivedClass>>(ctor);
            var derivedClass = func(0, 0);
            int value = (int)derivedClass.BaseValue;

            Assert.AreEqual(value, 104);
        }

        [TestMethod]
        public void ConstructorResolving_UsingBinaryExpressionOfSubtractionForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(string), typeof(string));
            var func = ExecuteConstructorLambda<Func<string, string, DerivedClass>>(ctor);
            var derivedClass = func(string.Empty, string.Empty);
            int value = (int)derivedClass.BaseValue;

            Assert.AreEqual(value, 96);
        }

        [TestMethod]
        public void ConstructorResolving_UsingBinaryExpressionOfMultiplicationForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(bool), typeof(bool));
            var func = ExecuteConstructorLambda<Func<bool, bool, DerivedClass>>(ctor);
            var derivedClass = func(true, true);
            int value = (int)derivedClass.BaseValue;

            Assert.AreEqual(value, 400);
        }

        [TestMethod]
        public void ConstructorResolving_UsingBinaryExpressionOfDivisionForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(char), typeof(char));
            var func = ExecuteConstructorLambda<Func<char, char, DerivedClass>>(ctor);
            var derivedClass = func('A', 'A');
            int value = (int)derivedClass.BaseValue;

            Assert.AreEqual(value, 25);
        }

        [TestMethod]
        public void ConstructorResolving_UsingBinaryExpressionOfModulusForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(byte), typeof(byte));
            var func = ExecuteConstructorLambda<Func<byte, byte, DerivedClass>>(ctor);
            var derivedClass = func(0, 0);
            int value = (int)derivedClass.BaseValue;

            Assert.AreEqual(value, 0);
        }
    }
}