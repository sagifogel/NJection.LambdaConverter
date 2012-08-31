using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests
{
    /// <summary>
    /// Summary description for Switch
    /// </summary>
    [TestClass]
    public class SwitchExpression : BaseTest
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
        public void SwitchStatementOfEnum_GivenASpecificValueOfTheEnum_ReturnsEnumsStringRepresentation()
        {
            ConsoleColor consoleColor = ConsoleColor.DarkCyan;
			Func<ConsoleColor, string> @delegate = GetColor;
			var func = ExecuteLambda<Func<ConsoleColor, string>>(@delegate);
            string color = func(consoleColor);

            Assert.AreEqual(color, consoleColor.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void SwitchStatementOfEnum_GivenASpecificValueWithoutCorrlatedCase_ThrowsNotImplementedException()
        {
			Func<ConsoleColor, string> @delegate = GetColor;
			var func = ExecuteLambda<Func<ConsoleColor, string>>(@delegate);

            func(ConsoleColor.Black);
        }

        [TestMethod]
        public void SwitchStatementOfEnum_WithNoCases_ReturnsEnumsStringRepresentatio()
        {
            ConsoleColor consoleColor = ConsoleColor.DarkCyan;
			Func<ConsoleColor, string> @delegate = GetColorWithNoCases;
			var func = ExecuteLambda<Func<ConsoleColor, string>>(@delegate);
            string color = func(consoleColor);

            Assert.AreEqual(color, consoleColor.ToString());
        }

        private string GetColor(ConsoleColor consoleColor)
        {
            string color = string.Empty;

            switch (consoleColor)
            {
                case ConsoleColor.DarkCyan:

                    color = consoleColor.ToString();
                    break;

                default:

                    throw new NotImplementedException();
            }

            return color;
        }

        private string GetColorWithNoCases(ConsoleColor consoleColor)
        {
            string color = string.Empty;

            switch (consoleColor)
            {
                default:

                    color = consoleColor.ToString();
                    break;
            }

            return color;
        }
    }
}
