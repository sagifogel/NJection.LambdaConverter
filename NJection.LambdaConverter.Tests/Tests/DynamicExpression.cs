using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests
{
    /// <summary>
    /// Summary description for DynamicExpression
    /// </summary>
    [TestClass]
    public class DynamicExpression : BaseTest
    {
        private TestContext testContextInstance;
           
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
        public void DynamicExpression_OfFieldReferenceGivenAConstantValueToInitilaize_ReturnsTheSameValueAfterInvokingTheCallSite()
        {
            string message = "dynamic invocation";
            Func<string, dynamic> @delegate = DynamicInvocationOfProperty;
			var func = ExecuteLambda<Func<string, dynamic>>(@delegate);
            var result = func(message);

            Assert.AreEqual(result, message);
        }

        [TestMethod]
        public void DynamicExpression_OfAnAssignmentForTheFirstMemeberOfAList_ReturnsDiffrentValueAfterInvokingTheCallSite()
        {
            List<string> strings = new List<string> { "dynamic invocation" };
            string beforeInvocation = strings[0];
            Action<List<string>> @delegate = DynamicAssignment;
			var func = ExecuteLambda<Action<List<string>>>(@delegate);

            func(strings);

            Assert.AreNotEqual(strings[0], beforeInvocation);
        }

        [TestMethod]
        public void DynamicExpression_OfAnAssignmentToListFromADynamicPropertyForTheFirstMemeberOfTheAList_ReturnsDiffrentValueAfterInvokingTheCallSite()
        {
            List<string> strings = new List<string> { "dynamic invocation" };
            string beforeInvocation = strings[0];
            Action<List<string>> @delegate = DynamicPropertyBinding;
			var func = ExecuteLambda<Action<List<string>>>(@delegate);

            func(strings);

            Assert.AreNotEqual(strings[0], beforeInvocation);
        }

        private dynamic DynamicInvocationOfProperty(string message)
        {
            dynamic dynamicException = new Exception(message);

            return dynamicException.Message;
        }

        private void DynamicAssignment(List<string> strings)
        {
            dynamic dynamicListOfString = strings;

            dynamicListOfString[0] = Guid.NewGuid().ToString();
        }

        private void DynamicPropertyBinding(List<string> strings)
        {
            dynamic dynamicListOfString = new Exception("Dynamic");

            strings[0] = dynamicListOfString.Message;
        }
    }
}
