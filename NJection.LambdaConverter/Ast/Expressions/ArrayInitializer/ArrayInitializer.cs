using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Cecil;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using System;
using NRefactory = ICSharpCode.NRefactory.CSharp;
using ICSharpCode.Decompiler.Ast;

namespace NJection.LambdaConverter.Expressions
{
    public class ArrayInitializer : AstExpression
    {
        private bool isPropertyAssignment = false;
        private ConstructorInfo _constructor = null;
        private List<Expression> _arguments = null;
        private List<AstExpression> _initializers = null;
        private NRefactory.ArrayInitializerExpression _arrayInitializerExpression = null;

        protected internal ArrayInitializer(NRefactory.ArrayInitializerExpression arrayInitializerExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            TypeInformation typeInformation;
            MethodReference methodReference;

            _arrayInitializerExpression = arrayInitializerExpression;

            if (_arrayInitializerExpression.Parent.HasAnnotationOf<MethodReference>(out methodReference)) {
                _constructor = methodReference.GetActualMethod() as ConstructorInfo;
                InternalType = _constructor.DeclaringType;

                if (_constructor.GetParameters().Length > 0) {
                    var newExpression = _arrayInitializerExpression.Parent as NRefactory.ObjectCreateExpression;

                    if (newExpression != null) {
                        _arguments = newExpression.Arguments
                                                  .Select(arg => arg.AcceptVisitor(Visitor, ParentScope).Reduce())
                                                  .ToList();
                    }
                }
            }
            else if (_arrayInitializerExpression.Parent.HasAnnotationOf<TypeInformation>(out typeInformation)) {
                ConstructorInfo[] ctors = null;

                InternalType = typeInformation.InferredType.GetActualType();
                ctors = InternalType.GetConstructors();

                if (ctors.Length > 1) {
                    _constructor = ctors[0];
                }
            }

            if (InternalType != null && !InternalType.IsGenericListOrDictionary() && !InternalType.IsArray) {
                isPropertyAssignment = true;
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
            if (!isPropertyAssignment) {
                if (IsComplexList()) {
                    return ReduceComplexList();
                }

                return Expression.ListInit(Expression.New(_constructor), Initializers);
            }

            return ReduceToMemberInitExpression();
        }

        private Expression ReduceToMemberInitExpression() {
            NewExpression newExpression = null;
            var bindings = _initializers.Cast<NamedExpression>()
                                       .Select(namedExpression => {
                                           var membersInfo = InternalType.GetMember(namedExpression.Name);
                                           var memberInfo = membersInfo[0];

                                           return Expression.Bind(membersInfo[0], namedExpression);
                                       });

            if (_constructor == null) {
                newExpression = Expression.New(InternalType);
            }
            else {
                var @params = _constructor.GetParameters();

                if (@params.Length == 0) {
                    newExpression = Expression.New(_constructor);
                }
                else {
                    if (_arguments == null) {
                        _arguments = @params.Select(p => {
                            var type = p.ParameterType;
                            var value = type.IsValueType ? Activator.CreateInstance(type) : null;

                            return (Expression)Expression.Constant(value, type);
                        }).ToList();
                    }

                    newExpression = Expression.New(_constructor, _arguments);
                }
            }

            return Expression.MemberInit(newExpression, bindings);
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