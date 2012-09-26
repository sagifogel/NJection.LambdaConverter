using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NJection.Core;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class MethodDeclaration : AbstractMethodDeclaration<NRefactory.MethodDeclaration>
    {
        protected internal MethodDeclaration(NRefactory.MethodDeclaration methodDeclaration, object context, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(methodDeclaration, context: context, scope: scope, visitor: visitor) { }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.MethodDeclaration; }
        }

        protected override Type GetReturnType() {
            return Declaration.ReturnType.GetActualType();
        }

        protected override NRefactory.BlockStatement GetBody() {
            return Declaration.Body;
        }

        protected override void ExtractContext() {
            if (ThisReference != null) {
                this.Context = new ConstantContext(ThisReference);
            }
        }

        protected override IEnumerable<ParameterExpression> ExtractParameters() {
            return Declaration.Parameters
                              .Select(p => p.AcceptVisitor(Visitor, this).Reduce())
                              .Cast<ParameterExpression>()
                              .Select(p => {
                                  VariablesStore.Add(p.Name, new Variable(p));
                                  return p;
                              })
                              .ToList();
        }

        public override Expression Reduce() {
            var delegateType = Parameters.Select(p => p.IsByRef ? p.Type.MakeByRefType() : p.Type)
                                         .Concat(Type);

            return Expression.Invoke(
                        Expression.Lambda(
                            Expression.GetDelegateType(delegateType.ToArray()),
                                Body,
                                    Parameters),
                                        Parameters);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitMethodDeclarationBlock(this);
        }

        public override Expression Update(Expression body, IEnumerable<ParameterExpression> parameters) {
            if (Body.Equals(body) && ReferenceEquals(Parameters, parameters)) {
                return this;
            }

            return AstExpression.Method(Declaration, ThisReference, scope: this, visitor: Visitor);
        }
    }
}