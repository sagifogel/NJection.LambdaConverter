using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.UnaryExpression
{
    /// <summary>
    /// Summary description for TypeAsExpression
    /// </summary>
    [TestClass]
    public class TypeAsExpression : BaseTest
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
        public void TypeAsExpression_FromASmallerMoreSpecificTypeToABiggerLessSpecificType_ReturnsNotNull()
        {
            Func<object> @delegate = CastFromRelatedTypes;
			var func = ExecuteLambda<Func<object>>(@delegate);
            var result = func();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TypeAsExpression_FromSpecificTypeThatIsUnrelatedToTheConvertedType_ReturnsNull()
        {
            Func<object> @delegate = CastFromUnrelatedTypes;
			var func = ExecuteLambda<Func<object>>(@delegate);
            var result = func();

            Assert.IsNull(result);
        }

        private object CastFromRelatedTypes()
        {
            IEnumerable<object> list = new List<object>();

            return list as IEnumerable<object>;
        }

        private object CastFromUnrelatedTypes()
        {
            IEnumerable<object> list = new List<object>();

            return list as IEnumerable<string>;
        }
    }
}
