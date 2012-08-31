using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests
{
    /// <summary>
    /// Summary description for Eumerators
    /// </summary>
    [TestClass]
    public class Eumerators : BaseTest
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
        public void IEnumeratorOfString_CreatedByYeildingTheResults_ReturnsTheSameSequenceOfIEnumeratorOfString()
        {
			Func<IEnumerator<string>> @delegate = IEnumerableUsingYeildReturnReturnsIEnumerator;
			var func = ExecuteLambda<Func<IEnumerator<string>>>(@delegate);
            var sequecne = func();

            Assert.IsTrue(SequenceEqual(new string[] { "!", "@", "#", "$", "%" }, sequecne));
        }

        [TestMethod]
        public void GenericIEnumeratorOfString_CreatedByInitializingANewList_ReturnsTheSameSequenceOfIEnumeratorOfString()
        {
			Func<IEnumerable<string>> @delegate = GenericIEnumerableCreatedByNewListReturnsIEnumerableOfSpecificType<string>;
			var func = ExecuteLambda<Func<IEnumerable<string>>>(@delegate);
            var sequecne = func();
            var @default = default(string);

            Assert.IsTrue(SequenceEqual(new string[] { @default, @default }, sequecne.GetEnumerator()));
        }

        public IEnumerable<T> GenericIEnumerableCreatedByNewListReturnsIEnumerableOfSpecificType<T>()
        {
            return new List<T> { default(T), default(T) };
        }

        private IEnumerator<string> IEnumerableUsingYeildReturnReturnsIEnumerator()
        {
            foreach (var item in new List<string> { "!", "@", "#", "$", "%" })
            {
                yield return item;
            }
        }

        private bool SequenceEqual(IEnumerable<string> source, IEnumerator<string> enumerator)
        {
            return source.All(e =>
            {
                if (enumerator.MoveNext())
                {
                    return e == enumerator.Current;
                }

                return false;
            });
        }
    }
}
