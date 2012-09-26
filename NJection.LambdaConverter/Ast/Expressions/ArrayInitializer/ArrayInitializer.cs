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
    public class ArrayInitializer : AstExpression
    {
        private ConstructorInfo _constructor = null;
        private List<AstExpression> _initializers = null;
        private NRefactory.ArrayInitializerExpression _arrayInitializerExpression = null;

        protected internal ArrayInitializer(NRefactory.ArrayInitializerExpression arrayInitializerExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            MethodReference methodReference;

            _arrayInitializerExpression = arrayInitializerExpression;

            if (_arrayInitializerExpression.Parent.HasAnnotationOf<MethodReference>(out methodReference)) {
                _constructor = methodReference.GetActualMethod() as ConstructorInfo;
                InternalType = _constructor.DeclaringType;
            }

            _initializers = arrayInitializerExpression.Elements
                                                      .Select(e => e.AcceptVisitor(Visitor, ParentScope))
                                                      .ToList();
        }

        public IEnumerable<Expression> Initializers {
            get { return _initializers; }
        }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.ArrayInitializer; }
        }

        public override Expression Reduce() {
            if (IsComplexList()) {
                return ReduceComplexList();
            }

            return Expression.ListInit(Expression.New(_constructor), Initializers);
        }

        private Expression ReduceComplexList() {
            var addMethod = InternalType.GetMethod("Add");
            var inits = _initializers.Cast<ArrayInitializer>()
                                     .Select(i => Expression.ElementInit(addMethod, i.Initializers));

            return Expression.ListInit(Expression.New(_constructor), inits);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitArrayInitializer(this);
        }

        public Expression Update(IEnumerable<Expression> Initializers) {
            if (Initializers.Equals(Initializers)) {
                return this;
            }

            return AstExpression.ArrayInitializer(_arrayInitializerExpression, ParentScope, Visitor);
        }

        private bool IsComplexList() {
            return _initializers[0] is ArrayInitializer;
        }
    }
}