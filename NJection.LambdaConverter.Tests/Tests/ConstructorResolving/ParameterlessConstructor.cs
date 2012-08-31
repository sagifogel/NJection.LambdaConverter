using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests.ConstructorResolving
{
    /// <summary>
    /// Summary description for ParametrlessConstructor
    /// </summary>
    [TestClass]
    public class ParameterlessConstructor : BaseConstructorTest
    {
        private TestContext testContextInstance;

        public class ParameterlessDerivedClass : BaseClass
        {
            public string Value { get; set; }

            public ParameterlessDerivedClass() {
            }
        }

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        [TestMethod]
        public void ConstructorResolving_UsingParameterlessConstructor_ReturnsNonProxiedInstance() {
            var ctor = GetConstructorByType<ParameterlessDerivedClass>(Type.EmptyTypes);
            var func = ExecuteConstructorLambda<Func<ParameterlessDerivedClass>>(ctor);
            var derivedClass = func();
            var proxiedType = derivedClass.GetType();
            var derivedClassType = typeof(ParameterlessDerivedClass);

            Assert.IsTrue(proxiedType.Equals(derivedClassType), string.Format("Types are not equivalent {0} != {1}", proxiedType.Name, derivedClassType.Name));
        }
    }
}