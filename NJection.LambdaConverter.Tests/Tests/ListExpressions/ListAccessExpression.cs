using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests
{
    /// <summary>
    /// Summary description for ListAccessExpression
    /// </summary>
    [TestClass]
    public class ListAccessExpression : BaseTest
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

        [TestMethod]
        public void ListAccess_OfAssignmentOfSpecificValueInReturnStatment_ReturnsTheAssignedValue()
        {
            int value = 10;
            Func<int, int, int> @delegate = ListWriteAccessReturnsAssignmentExpression;
			var func = ExecuteLambda<Func<int, int, int>>(@delegate);
            var result = func(0, value);

            Assert.AreEqual(result, value);
        }

        [TestMethod]
        public void ListAccess_OfIndexingOf_ReturnsTheIndexedValue()
        {
            int value = 10;
            Func<int, int> @delegate = ListReadAccessReturnsAssignmentExpression;
			var func = ExecuteLambda<Func<int, int>>(@delegate);
            var result = func(value);

            Assert.AreEqual(result, value);
        }
        
        private int ListWriteAccessReturnsAssignmentExpression(int index, int value)
        {
            List<int> ints = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            
            return ints[index] = value;
        }

        private int ListReadAccessReturnsAssignmentExpression(int index)
        {
            List<int> ints = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            return ints[index];
        }
    }
}
