using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests
{
    /// <summary>
    /// Summary description for Reflection
    /// </summary>
    [TestClass]
    public class Reflection : BaseTest
    {
        private class GenericClass<T>
        {
            public static T MyProperty
            {
                get { return default(T); }
            }
        }

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
        public void GenricTypePropertyInvocation_GivenASpecificType_ReturnsTheDefaultValueOfTheType()
        {
			Func<ConsoleColor> @delegate = GenericTypeDefaultValueWithReflection<ConsoleColor>;
			var func = ExecuteLambda<Func<ConsoleColor>>(@delegate);
            var consoleColor = func();

            Assert.AreEqual(consoleColor, ConsoleColor.Black);
        }

        public T GenericTypeDefaultValueWithReflection<T>()
        {
            Type gt = typeof(GenericClass<>);
            Type t = gt.MakeGenericType(new Type[] { typeof(T) });
            PropertyInfo p = t.GetProperty("MyProperty");

            return (T)p.GetGetMethod().Invoke(null, null);
        }
    }
}
