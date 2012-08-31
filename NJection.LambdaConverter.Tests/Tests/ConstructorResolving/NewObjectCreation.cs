using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.ConstructorResolving
{
    [TestClass]
    public class NewObjectCreation : BaseConstructorTest
    {
        private TestContext testContextInstance;

        public class DummyClass
        {
            public int Value { get; set; }

            public DummyClass(int value) {
                Value = value;
            }

            public DummyClass() { }
        }

        public class DummyStruct
        {
            public int Value { get; set; }

            public DummyStruct(int value) {
                Value = value;
            }

            public DummyStruct() { }
        }

        public class DerivedClass : BaseClass
        {
            public int Value { get; set; }

            public DerivedClass(DummyClass d)
                : base(new DummyClass(10)) {
                Value = 0;
            }

            public DerivedClass(int value)
                : base(new DummyClass { Value = value }) {
                Value = 0;
            }

            public DerivedClass(object value)
                : base(new { String = string.Empty, Boolean = true, ListOfStrings = new List<string> { "A", "B" } }) {
                Value = 0;
            }

            public DerivedClass()
                : base(new { }) {
                Value = 0;
            }

            public DerivedClass(DummyStruct dummy)
                : base(new DummyStruct { Value = 10 }) {
                Value = 0;
            }

            public DerivedClass(DummyStruct dummy, int i)
                : base(new DummyStruct(10) { Value = 20 }) {
                Value = 0;
            }

            public DerivedClass(DummyStruct dummy, double i)
                : base(new DummyStruct(10)) {
                Value = 0;
            }

            public DerivedClass(ArrayList arrayList)
                : base(new ArrayList { string.Empty, true, new List<string> { "A", "B" } }) {
                Value = 0;
            }

            public DerivedClass(List<object> list)
                : base(new List<object> { string.Empty, true, new List<string> { "A", "B" } }) {
                Value = 0;
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
        public void ConstructorResolving_UsingCreateNewObjectArgumentForBaseConstructor_ReturnsProxiedInstanceWithNewObject() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(DummyClass));
            var func = ExecuteConstructorLambda<Func<DummyClass, DerivedClass>>(ctor);
            var derivedClass = func(null);
            var dummy = derivedClass.BaseValue;

            Assert.IsInstanceOfType(dummy, typeof(DummyClass));
            Assert.IsTrue((dummy as DummyClass).Value.Equals(10));
        }

        [TestMethod]
        public void ConstructorResolving_UsingCreateNewObjectArgumentWithInitializersForBaseConstructor_ReturnsProxiedInstanceWithNewObject() {
            int argument = 10;
            var ctor = GetConstructorByType<DerivedClass>(typeof(int));
            var func = ExecuteConstructorLambda<Func<int, DerivedClass>>(ctor);
            var derivedClass = func(argument);
            var dummy = derivedClass.BaseValue as DummyClass;

            Assert.IsTrue(dummy.Value.Equals(argument));
        }

        [TestMethod]
        public void ConstructorResolving_UsingCreateNewAnonymousTypeArgumentWithInitializersForBaseConstructor_ReturnsProxiedInstanceWithNewObject() {
            int argument = 10;
            var ctor = GetConstructorByType<DerivedClass>(typeof(object));
            var func = ExecuteConstructorLambda<Func<object, DerivedClass>>(ctor);
            var derivedClass = func(argument);
            var dummy = derivedClass.BaseValue;
            dynamic dynamicDummy = dummy;

            Assert.IsTrue(dummy.GetType().IsDefined(typeof(CompilerGeneratedAttribute), true));
            Assert.IsTrue(dynamicDummy.String.Equals(string.Empty));
            Assert.IsTrue(dynamicDummy.Boolean);
            Assert.IsTrue((dynamicDummy.ListOfStrings as List<string>).SequenceEqual(new List<string> { "A", "B" }));
        }

        [TestMethod]
        public void ConstructorResolving_UsingCreateNewAnonymousTypeArgumentWithoutInitializersForBaseConstructor_ReturnsProxiedInstanceWithNewObject() {
            var ctor = GetConstructorByType<DerivedClass>(Type.EmptyTypes);
            var func = ExecuteConstructorLambda<Func<DerivedClass>>(ctor);
            var derivedClass = func();
            var dummy = derivedClass.BaseValue;

            Assert.IsTrue(dummy.GetType().IsDefined(typeof(CompilerGeneratedAttribute), true));
        }

        [TestMethod]
        public void ConstructorResolving_UsingCreateNewStructArgumentWithInitializersAndWithoutArgumentsForBaseConstructor_ReturnsProxiedInstanceWithNewStruct() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(DummyStruct));
            var func = ExecuteConstructorLambda<Func<DummyStruct, DerivedClass>>(ctor);
            var derivedClass = func(default(DummyStruct));
            var dummy = derivedClass.BaseValue;

            Assert.IsInstanceOfType(dummy, typeof(DummyStruct));
        }

        [TestMethod]
        public void ConstructorResolving_UsingCreateNewStructArgumentWithInitializersAndWithArgumentsForBaseConstructor_ReturnsProxiedInstanceWithNewStruct() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(DummyStruct), typeof(int));
            var func = ExecuteConstructorLambda<Func<DummyStruct, int, DerivedClass>>(ctor);
            var derivedClass = func(default(DummyStruct), 0);
            var dummy = derivedClass.BaseValue;

            Assert.IsInstanceOfType(dummy, typeof(DummyStruct));
        }

        [TestMethod]
        public void ConstructorResolving_UsingCreateNewStructArgumentWithoutIntializersAndWithArgumentsForBaseConstructor_ReturnsProxiedInstanceWithNewStruct() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(DummyStruct), typeof(double));
            var func = ExecuteConstructorLambda<Func<DummyStruct, double, DerivedClass>>(ctor);
            var derivedClass = func(default(DummyStruct), 0);
            var dummy = derivedClass.BaseValue;

            Assert.IsInstanceOfType(dummy, typeof(DummyStruct));
        }

        [TestMethod]
        public void ConstructorResolving_UsingCreateNewArrayListArgumentWithIntializersAndWithArgumentsForBaseConstructor_ReturnsProxiedInstanceWithArrayList() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(ArrayList));
            var func = ExecuteConstructorLambda<Func<ArrayList, DerivedClass>>(ctor);
            var derivedClass = func(null);
            var baseValue = derivedClass.BaseValue;
            var arrayList = baseValue as ArrayList;

            Assert.IsInstanceOfType(baseValue, typeof(ArrayList));
            Assert.AreEqual(arrayList[0], string.Empty);
            Assert.IsTrue((bool)arrayList[1]);
            Assert.IsTrue((arrayList[2] as List<string>).SequenceEqual(new List<string> { "A", "B" }));
        }

        [TestMethod]
        public void ConstructorResolving_UsingCreateNewListOfObjectsArgumentWithIntializersAndWithArgumentsForBaseConstructor_ReturnsProxiedInstanceWithArrayList() {
            var ctor = GetConstructorByType<DerivedClass>(typeof(List<object>));
            var func = ExecuteConstructorLambda<Func<List<object>, DerivedClass>>(ctor);
            var derivedClass = func(null);
            var baseValue = derivedClass.BaseValue;
            var arrayList = baseValue as List<object>;

            Assert.IsInstanceOfType(baseValue, typeof(List<object>));
            Assert.AreEqual(arrayList[0], string.Empty);
            Assert.IsTrue((bool)arrayList[1]);
            Assert.IsTrue((arrayList[2] as List<string>).SequenceEqual(new List<string> { "A", "B" }));
        }
    }
}