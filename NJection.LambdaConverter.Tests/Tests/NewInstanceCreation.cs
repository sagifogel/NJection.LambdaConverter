using System;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests
{
    [TestClass]
    public partial class NewInstanceCreation : BaseTest
    {
        public struct Target3
        {
            public int X;

            public Target3(int x) {
                X = x;
            }
        }

        public struct Target2
        {
            public Nullable<int> X;
        }

        public class Target
        {
            public Nullable<int> Property2 { get; set; }

            public Nullable<int> Property { get; private set; }

            public Target() { }

            public Target(int i) {
                Property = i;
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
        public void ConstructorInvocation_WithoutParemeters_ReturnsTargetWithNullProperty() {
            Func<Target> @delegate = CreateParameterlessInstanceOfTarget;
            var func = ExecuteLambda<Func<Target>>(@delegate);
            Assert.IsNull(func().Property);
        }

        [TestMethod]
        public void ConstructorInvocation_WithOneParmeterThatAssignsItToANullableProperty_ReturnsNotNullProperty() {
            Func<Target> @delegate = CreateInstanceOfTargetUsingArguments;
            var func = ExecuteLambda<Func<Target>>(@delegate);
            Assert.IsNotNull(func().Property);
        }

        [TestMethod]
        public void AnonymousTypeCreation_ReturnsCompilerGeneratedAttribute() {
            Func<object> @delegate = CreateAnonymousTypeInstance;
            var func = ExecuteLambda<Func<object>>(@delegate);
            Assert.IsTrue(func().GetType().IsDefined(typeof(CompilerGeneratedAttribute), true));
        }

        [TestMethod]
        public void EmptyAnonymousTypeCreation_ReturnsCompilerGeneratedAttribute() {
            Func<object> @delegate = CreateEmptyAnonymousTypeInstance;
            var func = ExecuteLambda<Func<object>>(@delegate);
            Assert.IsTrue(func().GetType().IsDefined(typeof(CompilerGeneratedAttribute), true));
        }

        [TestMethod]
        public void NewObjectCreation_WithInitilaizers_ReturnsNotNullProperty() {
            Func<object> @delegate = CreateInstanceOfTargetUsingInitilaizer;
            var func = ExecuteLambda<Func<Target>>(@delegate);
            Assert.IsNotNull(func().Property2);
        }

        [TestMethod]
        public void NewObjectCreation_OfStructWithInitilaizers_ReturnsNotNullField() {
            Func<Target2> @delegate = CreateInstanceOfTarget2StructUsingInitilaizer;
            var func = ExecuteLambda<Func<Target2>>(@delegate);
            Assert.IsNotNull(func().X);
        }

        [TestMethod]
        public void NewObjectCreation_OfStructWithoutInitilaizers_ReturnsNewInstanc() {
            Func<Target3> @delegate = CreateInstanceOfTarget3StructWithoutInitilaizer;
            var func = ExecuteLambda<Func<Target3>>(@delegate);
            Assert.AreEqual(func().GetType(), typeof(Target3));
        }

        public Target CreateParameterlessInstanceOfTarget() {
            return new Target();
        }

        public Target CreateInstanceOfTargetUsingArguments() {
            return new Target(10);
        }

        public Target CreateInstanceOfTargetUsingInitilaizer() {
            return new Target { Property2 = 10 };
        }

        public Target2 CreateInstanceOfTarget2StructUsingInitilaizer() {
            return new Target2 { X = 10 };
        }

        public Target3 CreateInstanceOfTarget3StructWithoutInitilaizer() {
            var target3 = new Target3();

            target3.X = 10;

            return target3;
        }

        public object CreateAnonymousTypeInstance() {
            return new { Message = "I am Anonymous", Message2 = "I am also Anonymous" };
        }

        public object CreateEmptyAnonymousTypeInstance() {
            return new { };
        }
    }
}
