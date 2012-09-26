using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Cecil;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class Index : AstExpression
    {
        private bool _isArrayIndex = false;
        private bool _isAssignment = false;
        private PropertyInfo _indexer = null;
        private NRefactory.IndexerExpression _indexerExpression = null;
        private Func<Expression, IEnumerable<Expression>, Expression> _methodCall = null;

        protected internal Index(NRefactory.IndexerExpression indexerExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            bool isAssignment = false;
            PropertyReference propertyReference = null;

            _indexerExpression = indexerExpression;
            isAssignment = IsAssignment();
            Target = indexerExpression.Target.AcceptVisitor(Visitor, ParentScope);
            TryGetArguments();

            if (indexerExpression.HasAnnotationOf<PropertyReference>(out propertyReference)) {
                var propertyInfo = Target.Type.GetProperty(propertyReference.Name);

                InternalType = propertyInfo.PropertyType;
                _indexer = propertyInfo;
            }
            else {
                _isArrayIndex = true;
                var targetType = Target.Type;

                if (targetType.HasElementType) {
                    targetType = Target.Type.GetElementType();
                }

                InternalType = targetType;
                _methodCall = _isAssignment ? Expression.ArrayAccess : (Func<Expression, IEnumerable<Expression>, Expression>)Expression.ArrayAccess;
            }
        }

        public Expression Target { get; private set; }

        public IEnumerable<Expression> Arguments { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Index; }
        }

        public override Expression Reduce() {
            if (_isArrayIndex) {
                return _methodCall(Target, Arguments);
            }

            return ReduceToMemeberAccess();
        }

        private Expression ReduceToMemeberAccess() {
            Expression target = null;

            if (!Target.Type.IsStatic()) {
                target = Target;
            }

            return Expression.MakeIndex(target, _indexer, Arguments);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitIndex(this);
        }

        public Expression Update(IEnumerable<Expression> arguments) {
            if (ReferenceEquals(Arguments, arguments)) {
                return this;
            }

            return AstExpression.Index(_indexerExpression, ParentScope, Visitor);
        }

        private bool IsAssignment() {
            return _indexerExpression.Role.ToString().Equals("left");
        }

        private void TryGetArguments() {
            if (_indexerExpression.Arguments.Count > 0) {
                Arguments = _indexerExpression.Arguments.Select(a => a.AcceptVisitor(Visitor, ParentScope));
            }
        }
    }
}