using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NJection.LambdaConverter.Visitors;
using NJection.LambdaConverter.Extensions;
using NRefactory = ICSharpCode.NRefactory.CSharp;
using NJection.LambdaConverter.DynamicProxies;

namespace NJection.LambdaConverter.Expressions
{
    public class ConstructorBlock : Block
    {
        private IEnumerable<ParameterExpression> _baseConstructorParameters = null;

        public ConstructorBlock(NRefactory.BlockStatement blockStatement,
                                ParameterExpression contextParameter,
                                IScope scope,
                                INRefcatoryExpressionVisitor visitor,
                                IEnumerable<ParameterExpression> parameters,
                                IEnumerable<ParameterExpression> baseConstructorParameters)
            : base(blockStatement, parameters: parameters, scope: scope, visitor: visitor) {

            _baseConstructorParameters = baseConstructorParameters;
            AddNewContextCreationExpression(contextParameter);
            InternalType = contextParameter.Type;
            AddReturnLabelExpression();
        }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.ConstructorBlock; }
        }

        public void AddNewContextCreationExpression(ParameterExpression contextParameter) {
            ConstructorInfo ctor = null;
            Type[] parameterTypes = null;
            Expression assignExpression = null;
            NewExpression newExpression = null;
            IEnumerable<Expression> parametersExpressions = null;
            
            if (!_baseConstructorParameters.IsNullOrEmpty()) {
                ParameterExpression parameter;
                List<Type> types = new List<Type>();
                var parametersSet = _baseConstructorParameters.ToDictionary(p => p.Name, p => p);

                parametersExpressions = Parameters.Select(p => {
                    Type type = p.Type;

                    if (parametersSet.TryGetValue(p.Name, out parameter)) {
                        type = parameter.Type.MakeByRefType();
                    }

                    types.Add(type);

                    return p;
                }).ToList();

                if (parametersSet.TryGetValue("Uniquifier", out parameter)) {
                    var expression = Expression.Default(parameter.Type);
                    
                    types.Add(parameter.Type);
                    parametersExpressions = parametersExpressions.Concat(expression);
                }

                parameterTypes = types.ToArray();
            }
            else {
                var types = Parameters.Select(p => p.IsByRef ? p.Type.MakeByRefType() : p.Type);
                parameterTypes = types.ToArray();
                parametersExpressions = Parameters;
            }

            ctor = contextParameter.Type.GetConstructor(parameterTypes);
            newExpression = Expression.New(ctor, parametersExpressions);
            assignExpression = Expression.Assign(contextParameter, newExpression);
            BlockExpressions.Insert(0, assignExpression);
        }

        public override Expression Reduce() {
            return base.Reduce();
        }
    }
}