using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions.Arithmetic
{
    internal sealed class CheckedUncheckedExpressions
    {
        private readonly Dictionary<NRefactory.BinaryOperatorType, CheckedUncheckedEntry> _methods = null;

        internal CheckedUncheckedExpressions() {
            _methods = new Dictionary<NRefactory.BinaryOperatorType, CheckedUncheckedEntry>()
            {
                { NRefactory.BinaryOperatorType.Add, new CheckedUncheckedEntry(Expression.Add, Expression.AddChecked) },
                { NRefactory.BinaryOperatorType.Subtract, new CheckedUncheckedEntry(Expression.Subtract, Expression.SubtractChecked) },
                { NRefactory.BinaryOperatorType.Multiply, new CheckedUncheckedEntry(Expression.Multiply, Expression.MultiplyChecked) }
            };
        }

        internal Func<Expression, Expression, BinaryExpression> this[NRefactory.BinaryOperatorType @operator, bool isChecked] {
            get {
                CheckedUncheckedEntry entry;
                Func<Expression, Expression, BinaryExpression> func = null;

                if (_methods.TryGetValue(@operator, out entry)) {
                    func = entry[isChecked];
                }

                return func;
            }
        }

        internal bool TryGetValue(NRefactory.BinaryOperatorType @operator, out CheckedUncheckedEntry entry) {
            return _methods.TryGetValue(@operator, out entry);
        }
    }
}