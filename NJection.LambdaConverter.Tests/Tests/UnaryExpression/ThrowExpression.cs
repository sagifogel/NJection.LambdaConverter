using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.UnaryExpression
{
    /// <summary>
    /// Summary description for Exceptions
    /// </summary>
    [TestClass]
    public class ThrowExpression : BaseTest
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
        [ExpectedException(typeof(NotSupportedException))]
        public void ThrowExpression_WithoutTryCatchBlock_ThrowsException()
        {
			Action @delegate = ThrowNotSupportedException;
            var func = ExecuteLambda<Action>(@delegate);
            func();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void ThrowExpression_InsideATryFinallyBlock_ThrowsTheException()
        {
			Action @delegate = TryFinallyBlockThrowNotSupportedException;
            var func = ExecuteLambda<Action>(@delegate);
            func();
        }

        [TestMethod]
        [ExpectedException(typeof(DivideByZeroException))]
        public void RethrowExpression_InsideACatchBlock_RethrowsTheException()
        {
			Action @delegate = TryCatchBlockWithARethrowStatment;
            var func = ExecuteLambda<Action>(@delegate);
            func();
        }

        [TestMethod]
        [ExpectedException(typeof(DivideByZeroException))]
        public void ThrowExpression_InsideACatchBlockWithAnExcptionVariable_ThrowsTheException()
        {
            Action @delegate = TryCatchBlockWithThrowExceptionAndExceptionVariable;
			var func = ExecuteLambda<Action>(@delegate);
            func();
        }

        [TestMethod]
        [ExpectedException(typeof(DivideByZeroException))]
        public void RethrowExpression_InsideASpecificCatchHandlerOfATryBlockWithMultipuleCatchHandlers_RethrowsException()
        {
            Action @delegate = TryCatchBlockWithAThrowExceptionAndMultipuleCatchHandlers;
			var func = ExecuteLambda<Action>(@delegate);
            func();
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void RethrowException_InANestedTryCatchBlockInsideTheOuterCatchBlock_RethrowsTheInnermostRaisedException()
        {
            Action @delegate = NestedTryCatchBlocksInOuterCatchBlock;
			var func = ExecuteLambda<Action>(@delegate);
            func();
        }

        private void ThrowNotSupportedException()
        {
            throw new NotSupportedException();
        }

        private void TryFinallyBlockThrowNotSupportedException()
        {
            try
            {
                throw new NotSupportedException();
            }
            finally
            {
            }
        }

        private void TryCatchBlockWithARethrowStatment()
        {
            try
            {
                throw new DivideByZeroException();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void TryCatchBlockWithThrowExceptionAndExceptionVariable()
        {
            try
            {
                throw new DivideByZeroException();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void TryCatchBlockWithAThrowExceptionAndMultipuleCatchHandlers()
        {
            try
            {
                throw new DivideByZeroException();
            }
            catch (DivideByZeroException)
            {
                throw;
            }
            catch (Exception)
            {
            }
        }

        private void NestedTryCatchBlocksInOuterCatchBlock()
        {
            try
            {
                throw new DivideByZeroException();
            }
            catch (Exception ex)
            {
                try
                {
                    if (ex is DivideByZeroException)
                    {
                        throw new NotImplementedException();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
