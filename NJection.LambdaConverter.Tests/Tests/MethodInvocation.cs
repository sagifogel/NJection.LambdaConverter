using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NJection.LambdaConverter.Tests
{
    [TestClass]
    public partial class MethodInvocation : BaseTest
    {
        public enum Month
        {
            Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec
        }

        delegate void TryParseOfMonth(string value, out Month month);
        delegate void TryParseOfMonth2(string value, ref Month month);
        delegate string ParamsDelegate(int index, params string[] strings);

        [TestMethod]
        public void MethodCallInvocation_UsingOutParametersInDelegeateSignature_UpdatesTheCorrectValue()
        {
            Month month;
            TryParseOfMonth @delegate = TryParseInvocation;
			var func = ExecuteLambda<TryParseOfMonth>(@delegate);

            func("Jun", out month);

            Assert.AreEqual(month, Month.Jun);
        }

        [TestMethod]
        public void MethodCallInvocation_UsingRefOutParametersInDelegeateSignature_UpdatesTheCorrectValue()
        {
            Month month = default(Month);
            TryParseOfMonth2 @delegate = TryParseInvocation2;
			var func = ExecuteLambda<TryParseOfMonth2>(@delegate);

            func("Jun", ref month);

            Assert.AreEqual(month, Month.Jun);
        }

        [TestMethod]
        public void GenericMethodCallInvocation_UsingRefParametersInDelegeateSignature_UpdatesTheCorrectValue()
        {
            Month month;
            TryParseOfMonth @delegate = TryParseInvocationUsingGenerics;
			var func = ExecuteLambda<TryParseOfMonth>(@delegate);

            func("Jun", out month);

            Assert.AreEqual(month, Month.Jun);
        }

        [TestMethod]
        public void GenericMethodCallInvocation_WithGenericArgumentForBothInvocationAndReturnType_ReturnsTheCorrectValue()
        {
            Month month;
			Func<string, Month> @delegate = TryParseInvocationReturnsGenericValue<Month>;
			var func = ExecuteLambda<Func<string, Month>>(@delegate);

            month = func("Mar");

            Assert.AreEqual(month, Month.Mar);
        }

        [TestMethod]
        public void GenericMethodCallInvocation_WithTwoGenericArgumentsWhichOneOfThemIsTheReturnType_ReturnsCorrectInstanceOfTheReturnType()
        {   
            object @object;
			Func<object> @delegate = TwoGenericArgumentsInvocation<int, object>;
			var func = ExecuteLambda<Func<object>>(@delegate);

            @object = func();

            Assert.IsInstanceOfType(@object, typeof(object));
        }

        [TestMethod]
        public void MethodCallInvocation_WithParamsArgument_ReturnsTheLastElementOfTheArray()
        {
            ParamsDelegate @delegate = MethodWithParams;
			var func = ExecuteLambda<ParamsDelegate>(@delegate);
            string month = func(5, Month.Jan.ToString(), Month.Feb.ToString(), Month.Mar.ToString(), Month.Apr.ToString(), Month.May.ToString(), Month.Jun.ToString());

            Assert.AreEqual(month, Month.Jun.ToString());
        }

        public void TryParseInvocation(string value, out Month result)
        {
            Enum.TryParse<Month>(value, true, out result);
        }

        public void TryParseInvocation2(string value, ref Month result)
        {
            Enum.TryParse<Month>(value, true, out result);
        }

        public void TryParseInvocationUsingGenerics<T>(string value, out T result)
            where T : struct
        {
            Enum.TryParse<T>(value, true, out result);
        }

        public T TryParseInvocationReturnsGenericValue<T>(string value)
            where T : struct
        {
            T result;

            Enum.TryParse<T>(value, true, out result);

            return result;
        }

        public T2 TwoGenericArgumentsInvocation<T, T2>()
            where T : struct
            where T2 : new()
        {
            ValueType valueType = default(T);
            return new T2();
        }


        public string MethodWithParams(int index, params string[] objects)
        {
            return objects[index];
        }
    }
}
