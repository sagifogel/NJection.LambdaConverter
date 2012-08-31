using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests
{
    /// <summary>
    /// Summary description for Linq
    /// </summary>
    [TestClass]
    public class Linq : BaseTest
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
        public void RangeOfIntsStartingFromOne_GivenASpecificNumber_ReturnsTheResultOfFactorialOperation()
        {
			Func<int, double> @delegate = Factorial;
			var func = ExecuteLambda<Func<int, double>>(@delegate);
            double result = func(6);
            Assert.AreEqual(result, 720);
        }

        [TestMethod]
        public void LinqRangeOfInts_WithLetExpressionOfPowerInTwo_ReturnsTheSameSequence()
        {
			Func<IEnumerable<int>> @delegate = Let;
			var func = ExecuteLambda<Func<IEnumerable<int>>>(@delegate);
            var sequecne = func();
            Assert.IsTrue(sequecne.SequenceEqual((new[] { 0, 1, 4, 9, 16, 25, 36, 49, 64, 81 })));
        }

        [TestMethod]
        public void ListOfInts_UsingMethodChaining_ReturnsSameSequence()
        {
			Func<IEnumerable<string>> @delegate = MethodCahaining;
			var func = ExecuteLambda<Func<IEnumerable<string>>>(@delegate);
            var sequecne = func();
            Assert.IsTrue(sequecne.SequenceEqual((new[] { "4", "9", "16", "25" })));
        }

        public double Factorial(int num)
        {
            List<int> ints = Enumerable.Range(1, num).ToList();

            return ints.Aggregate(1, (i, j) => i * j);
        }

        public IEnumerable<string> MethodCahaining()
        {
            List<int> ints = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            return ints.Where(i => i < 6)
                       .Select(i => Math.Pow(i, 2))
                       .Skip(1)
                       .Select(i => i.ToString());
        }

        public IEnumerable<int> Let()
        {
            return from i in Enumerable.Range(0, 10)
                   let itag = (int)Math.Pow(i, 2)
                   select itag;
        }
    }
}
