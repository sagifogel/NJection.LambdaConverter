using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests
{
    /// <summary>
    /// Summary description for ExpressionTrees
    /// </summary>
    [TestClass]
    public class ExpressionTrees : BaseTest
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
        public void LambdaExpressionCreation_ThatHasToParseBooleanValuesGivenSpecificStringRepresentationOfABoolean_ReturnsTheCorrectValueAfterCompilationAndInvocation()
        {
			Func<string, bool> @delegate = InvokeParseBoolLamabdaExpression;
			var func = ExecuteLambda<Func<string, bool>>(@delegate);
            var @bool = func("true");

            Assert.IsTrue(@bool);
        }

        
        [TestMethod]
        [Fixed("https://github.com/sagifogel/NJection.LambdaConverter/issues/1")]
        public void LambdaExpresionLCreaton_ThatCreatesAnArrayAndQueriesItsLengthProperty_ReturnsTheCorrectValueOTheArrayLength() {
            Func<int> delegat = () => new int[0].Length;
            var result = ExecuteLambda(delegat);

            Assert.AreEqual(result(), 0);
        }

        private bool InvokeParseBoolLamabdaExpression(string val)
        {
            Expression<Func<string, bool>> ex = (value) => bool.Parse(value);
            return ex.Compile()(val);
        }
    }
}
