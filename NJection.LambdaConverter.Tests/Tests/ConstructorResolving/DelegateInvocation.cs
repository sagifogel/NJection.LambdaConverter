using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.ConstructorResolving
{
    [TestClass]
    public class DelegateInvocation : BaseConstructorTest
    {
        private int _ten = 10;

        public class DerivedClass : BaseClass
        {
            public delegate int _Delegate(string i);
            public static event _Delegate MyFunc3 = null;
            public static Func<double, int> MyFunc2 = null;
            public static Func<int, int> MyFunc { get; set; }

            public DerivedClass(double value) : base(MyFunc2(value)) { }
            public DerivedClass(int value) : base(MyFunc(value)) { }
            public DerivedClass(string value) : base(MyFunc3(value)) { }

            public static int MyFunc2Target(double i) {
                return (int)i;
            }

            public static int MyFuncTarget(int i) {
                return i;
            }

            public static int MyFunc3Target(string i) {
                return int.Parse(i);
            }
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

        [ClassInitialize()]
        public static void BindEvents(TestContext testContext) {
            DerivedClass.MyFunc = DerivedClass.MyFuncTarget;
            DerivedClass.MyFunc2 = DerivedClass.MyFunc2Target;
            DerivedClass.MyFunc3 += DerivedClass.MyFunc3Target;
        }

        #endregion

        [TestMethod]
        public void ConstructorResolving_UsingInvocationOfDelegateStoredInFieldForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(int));
            var func = ExecuteConstructorLambda<Func<int, DerivedClass>>(ctor);
            var derivedClass = func(_ten);

            Assert.AreEqual(derivedClass.BaseValue, _ten);
        }

        [TestMethod]
        public void ConstructorResolving_UsingInvocationOfDelegateStoredInPropertyForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(double));
            var func = ExecuteConstructorLambda<Func<double, DerivedClass>>(ctor);
            var derivedClass = func(_ten);

            Assert.AreEqual(derivedClass.BaseValue, _ten);
        }

        [TestMethod]
        public void ConstructorResolving_UsingInvocationOfAnEventForBaseConstructor_ReturnsCorrectValueInBaseClass() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(string));
            var func = ExecuteConstructorLambda<Func<string, DerivedClass>>(ctor);
            var derivedClass = func(_ten.ToString());

            Assert.AreEqual(derivedClass.BaseValue, _ten);
        }
    }
}
