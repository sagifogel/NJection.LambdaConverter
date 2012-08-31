using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;
using NJection.LambdaConverter.DynamicProxies;

namespace NJection.LambdaConverter.Expressions
{
    public class ConstructorDeclaration : AbstractMethodDeclaration<NRefactory.ConstructorDeclaration>
    {
        public ParameterExpression ContextParameter = null;

        internal ConstructorDeclaration(NRefactory.ConstructorDeclaration constructorDeclaration,
                                        Type proxyType,
                                        IEnumerable<Parameter> baseConstructorParameters,
                                        IScope scope,
                                        INRefcatoryExpressionVisitor visitor)
            : base(constructorDeclaration, scope, visitor, type: proxyType, baseConstructorParameters: baseConstructorParameters) {
        }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.ConstructorDeclaration; }
        }

        protected override Expression CreateBody(NRefactory.BlockStatement blockStatement, IEnumerable<ParameterExpression> parameters, IScope scope, INRefcatoryExpressionVisitor visitor) {
            IEnumerable<ParameterExpression> baseExpressionsParameters = null;

            if (!BaseConstructorParameters.IsNullOrEmpty()) {
                baseExpressionsParameters = BaseConstructorParameters.Select(p => Expression.Parameter(p.Type, p.Name));
            }

            return AstExpression.ConstructorBlock(blockStatement, ContextParameter, parameters: parameters, scope: scope, visitor: visitor, baseConstructorParameters: baseExpressionsParameters);
        }

        public override Expression Reduce() {
            Type[] delegateType = new Type[Declaration.Parameters.Count + 2];
            List<ParameterExpression> parameters = Parameters.ToList();

            Parameters.Select(p => p.IsByRef ? p.Type.MakeByRefType() : p.Type)
                      .ForEach(1, (type, i) => delegateType[i] = type);

            delegateType[0] = delegateType[delegateType.Length - 1] = Type;
            parameters.Insert(0, ContextParameter);

            return Expression.Invoke(
                        Expression.Lambda(
                            Expression.GetDelegateType(delegateType.ToArray()),
                                Body,
                                  parameters),
                                    parameters);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitConstructorDeclaration(this);
        }

        public override Expression Update(Expression body, IEnumerable<ParameterExpression> parameters) {
            if (Body.Equals(body) && ReferenceEquals(Parameters, parameters)) {
                return this;
            }

            return AstExpression.Constructor(Declaration, Type, BaseConstructorParameters, Visitor, this);
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

        protected override Type GetReturnType() {
            return TypeOfThis;
        }

        protected override NRefactory.BlockStatement GetBody() {
            return Declaration.Body;
        }

        protected override void ExtractContext() {
            ContextParameter = Expression.Parameter(TypeOfThis, "this");
            this.Context = new ParameterContext(ContextParameter);
        }
    }
}