using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Visitors;

namespace NJection.LambdaConverter.Expressions
{
    public class CompilerGeneratedEnumerator : Method
    {
        private int _stateInitializer = 0;
        private Type _declaringType = null;
        private Instruction _newObjInstruction = null;
        private Instruction _stateInitializerInstruction = null;

        public CompilerGeneratedEnumerator(Instruction stateInitializer, Instruction newObj, INRefcatoryExpressionVisitor visitor = null, IScope scope = null)
            : base(scope: scope, visitor: visitor) {
            var methodReference = newObj.Operand as MethodReference;

            if (stateInitializer.Operand != null) {
                _stateInitializer = int.Parse(stateInitializer.Operand.ToString()); // -2 | 0
            }

            _declaringType = methodReference.DeclaringType.GetActualType();
            _stateInitializerInstruction = stateInitializer;
            _newObjInstruction = newObj;
            InternalType = _declaringType;
            BuildExpression();
        }

        public Expression Enumerator { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.CompilerGeneratedEnumerator; }
        }

        public override Expression Reduce() {
            ParameterExpression variable = Expression.Variable(_declaringType);

            Parameters = Enumerable.Empty<ParameterExpression>()
                                   .Concat(variable);

            return Expression.Block(Parameters,
                                     variable,
                                        Expression.Assign(variable, Enumerator),
                                            variable);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitCompilerGeneratedEnumerator(this);
        }

        public Expression Update(Expression enumerator) {
            if (Enumerator.Equals(enumerator)) {
                return this;
            }

            return AstExpression.CompilerGeneratedEnumerator(_stateInitializerInstruction, _newObjInstruction, this, Visitor);
        }

        private void BuildExpression() {
            Expression stateInitializer = Expression.Constant(_stateInitializer);
            ConstructorInfo ctor = _declaringType.GetConstructor(new Type[] { TypeSystem.Int });

            Enumerator = Expression.New(ctor, stateInitializer);
        }
    }
}