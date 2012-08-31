using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.ListExpressions
{
    /// <summary>
    /// Summary description for ListInitExpression
    /// </summary>
    [TestClass]
    public class ListInitExpression : BaseTest
    {
        private TestContext testContextInstance;
        private readonly List<int> _ints = new List<int> { 1, 2, 3, 4, 5 };

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
        public void ListInit_OfSimpleListOfIntsWithPrimitiveInitializers_returnsTheCorrectSequence()
        {
            Func<List<int>> @delegate = SimpleListInitCreationWithInitializers;
			var func = ExecuteLambda<Func<List<int>>>(@delegate);
            var result = func();

            Assert.IsTrue(_ints.SequenceEqual(result));
        }

        [TestMethod]
        public void ListInit_OfDictionaryConsistsOfAnIntegerAsKeyAndListOfIntsAsValuesWithOneInitializer_ReturnsTheFirstValueAsTheCorrectSequenceAfterCreationAndInitialization()
        {
            Func<List<int>> @delegate = ComplexListInitCreationWithInitializers;
			var func = ExecuteLambda<Func<List<int>>>(@delegate);
            var result = func();

            Assert.IsTrue(_ints.SequenceEqual(result));
        }

        private List<int> SimpleListInitCreationWithInitializers()
        {
            return new List<int> { 1, 2, 3, 4, 5 };
        }

        private List<int> ComplexListInitCreationWithInitializers()
        {
            var ints = new Dictionary<int, List<int>> { { 0, new List<int> { 1, 2, 3, 4, 5 } } };

            return ints.Values.First();
        }
    }
}
