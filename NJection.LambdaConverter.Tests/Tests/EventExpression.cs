using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests
{
    /// <summary>
    /// Summary description for AddEventExpression
    /// </summary>
    [TestClass]
    public class EventExpression : BaseTest
    {
        private TestContext testContextInstance;

        private static event Func<int> _staticEvent;
        private ClassWithEvent _instance = new ClassWithEvent();

        public class ClassWithEvent
        {
            private event Action<int> _instanceEvent = null;

            public bool IsEventNull
            {
                get { return _instanceEvent == null; }
            }

            public void MethodBinder()
            {
                _instanceEvent += new Action<int>(DoNothing);
            }

            public void LambdaExpressionBinder()
            {
                _instanceEvent += new Action<int>((i) => { });
            }

            public void Clear()
            {
                MethodDisposer();
                _instanceEvent = null;
            }

            public void MethodDisposer()
            {
                _instanceEvent -= new Action<int>(DoNothing);
            }

            public void DoNothing(int i) { }
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


        #endregion

        [TestCleanup()]
        public void EventExpressionCleanUp()
        {
            Clear();
            _instance.Clear();
        }

        [TestMethod]
        public void AddEventExpression_OfStaticMethodToAGivenAStaticEventField_AssuresThatTheFiledEventHasBeenBinded()
        {
            Action action = StaticEventBinder;
            ExecuteLambda<Action>(action).Invoke();

            Assert.IsNotNull(_staticEvent);
        }

        [TestMethod]
        public void AddEventExpression_OfLambdaExpressionToAGivenAStaticEventField_AssuresThatTheFiledEventHasBeenBinded()
        {
            Action @delegate = StaticEventLambdaExpressionBinder;
            ExecuteLambda<Action>(@delegate).Invoke();

            Assert.IsNotNull(_staticEvent);
        }

        [TestMethod]
        public void AddEventExpression_GivenAnInstanceEventField_AssuresThatTheFieldEventHasBeenBinded()
        {
			Action @delegate = _instance.MethodBinder;
            ExecuteLambdaWithContext<Action, ClassWithEvent>(@delegate, _instance).Invoke();

            Assert.IsFalse(_instance.IsEventNull);
        }

        [TestMethod]
        public void AddEventExpression_ToLambdaExpressionGivenAnInstanceEventField_AssuresThatTheFieldEventHasBeenBinded()
        {
            Action @delegate = _instance.LambdaExpressionBinder;
            ExecuteLambdaWithContext<Action, ClassWithEvent>(@delegate, _instance).Invoke();

            Assert.IsFalse(_instance.IsEventNull);
        }

        [TestMethod]
        public void AddEventExpression_InvocatingTheStaticFieldEvent_ReturnsTheCorrectValue()
        {
			Func<int> @delegate = StaticEventBinderOfAFunction;
			var func = ExecuteLambda<Func<int>>(@delegate);
            var result = func();

            Assert.AreEqual(result, int.MaxValue);
        }

        [TestMethod]
        public void RemoveEventExpression_InvocatingTheStaticFieldEvent_AssuresThatTheFieldEventHasRemoved()
        {	
			StaticEventBinder();
			Action @delegate = StaticEventDisposer;
            ExecuteLambdaWithContext<Action, EventExpression>(@delegate, this).Invoke();

            Assert.IsNull(_staticEvent);
        }

        [TestMethod]
        public void RemoveEventExpression_GivenAnInstanceEventField_AssuresThatTheFieldEventHasRemoved()
        {
            _instance.MethodBinder();
            Action @delegate = _instance.MethodDisposer;
            ExecuteLambdaWithContext<Action, ClassWithEvent>(@delegate, _instance).Invoke();

            Assert.IsTrue(_instance.IsEventNull);
        }

        public void StaticEventBinder()
        {
            _staticEvent += new Func<int>(TargetActionToStaticEvent);
        }

        public void StaticEventDisposer()
        {
            _staticEvent -= new Func<int>(TargetActionToStaticEvent);
        }

        public int StaticEventBinderOfAFunction()
        {
            _staticEvent += new Func<int>(TargetActionToStaticEvent);
            return _staticEvent();
        }

        public void StaticEventLambdaExpressionBinder()
        {
            _staticEvent += new Func<int>(() => int.MaxValue);
        }

        private void TargetActionToInstanceMyEvent(int obj) { }

        private int TargetActionToStaticEvent()
        {
            return int.MaxValue;
        }

        private void Clear()
        {
            _staticEvent -= new Func<int>(TargetActionToStaticEvent);
            _staticEvent = null;
        }
    }
}
