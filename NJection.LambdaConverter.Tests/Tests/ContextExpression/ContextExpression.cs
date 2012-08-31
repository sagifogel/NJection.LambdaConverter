using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.ContextExpression
{
    /// <summary>
    /// Summary description for ContextExpression
    /// </summary>
    [TestClass]
    public class ContextExpression : BaseTest
    {
        private TestContext testContextInstance;

        class DerivedClass : BaseClass
        {
            public override int Value { get; set; }

            public override int GetValue()
            {
                return this.Value + base.Value;
            }

            public void SetBaseValue(int i)
            {
                base.Value = i;
            }

            public int GetBaseValue()
            {
                return base.Value;
            }
        }

        public class BaseClass
        {
            private int _value = 10;
            public virtual int Value { get { return _value; } set { _value = value;} }

            public virtual int GetValue()
            {
                return Value;
            }
        }

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
        public void SimpleContextExpression_InvocationOfAMethodThatReturnsTheValueOfASpecificPropertyOfTheInstance_ReturnsTheCorrectValueOfTheProperty()
        {
            BaseClass @base = new BaseClass();
			Func<int> @delegate = @base.GetValue;
            var func = ExecuteLambdaWithContext<Func<int>, BaseClass>(@delegate, @base);
            var result = func();

            Assert.AreEqual(result, 10);
        }

        [TestMethod]
        public void ComplexContextExpression_InvocationOfAVirtualMethodThatReturnsTheValueOfASpecificOverridablePropertyPlusTheBaseClassProperty_ReturnsTheCorrectAddedValue()
        {
            DerivedClass derived = new DerivedClass() { Value = 15 };
            Func<int> @delegate = derived.GetValue;
            var func = ExecuteLambdaWithContext<Func<int>, DerivedClass>(@delegate, derived);
            var result = func();

            Assert.AreEqual(result, 25);
        }

        [TestMethod]
        public void ComplexContextExpression_OfAssignmentToAPropertyOfTheBaseClass_ReturnsZeroWhenThePropertyOfTheDerivedClassIsInvoked()
        {
            int value = 15;
            DerivedClass derived = new DerivedClass();
            Action<int> @delegate = derived.SetBaseValue;
            var func = ExecuteLambdaWithContext<Action<int>, DerivedClass>(@delegate, derived);
            
            func(value);

            Assert.IsTrue(derived.Value == 0 && derived.GetBaseValue() == value);
        }
    }
}
