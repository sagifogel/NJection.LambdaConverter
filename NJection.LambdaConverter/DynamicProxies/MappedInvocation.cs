using System;
using System.Linq;
using System.Collections.Generic;
using NJection.LambdaConverter.Extensions;
using System.Reflection;
using Mono.Cecil;
using NRefactory = ICSharpCode.NRefactory.CSharp;
using NJection.LambdaConverter.Ast.Visitors;
using Mono.CSharp;
using ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.DynamicProxies
{
    public class MappedInvocation
    {
        private Parameter[] _parameters = null;
        private List<Parameter> _outParameters = null;
        private ParameterDeclaration[] _parameterDeclarations = null;
        private OutParametersMatchingVisitor _visitor = new OutParametersMatchingVisitor();

        public MappedInvocation(NRefactory.ConstructorDeclaration ctor, MethodDefinition baseCtor) {
            var parameterTypes = new List<Type>();
            var ctorType = ctor.GetActualType();

            _parameters = ctor.Parameters
                              .Select((p, i) => {
                                  return new Parameter(p.Name, p.GetActualType(), i);
                              })
                              .ToArray();

            _parameterDeclarations = ctor.Parameters.ToArray();
            _visitor.VisitConstructorDeclaration(ctor, null);
            _outParameters = new List<Parameter>();
            parameterTypes = new List<Type>();

            _parameters.ForEach((p, i) => {
                var type = p.Type;

                if (_visitor.Contains(p.Name)) {
                    if (!type.IsByRef) {
                        type = type.MakeByRefType();
                        p = new Parameter(p.Name, type, p.Location);
                    }

                    _outParameters.Add(p);
                }

                parameterTypes.Add(type);
            });

            VerifyUniqueness(ctorType, parameterTypes);
            Parameters = parameterTypes.ToArray();
        }

        public Type[] Parameters { get; private set; }

        public IEnumerable<Parameter> OutParameters {
            get {
                return _outParameters;
            }
        }

        public bool CanMapTo(NRefactory.ConstructorDeclaration ctor) {
            if (ctor.Parameters.Count != _parameterDeclarations.Length) {
                return false;
            }

            return ctor.Parameters
                       .All((p, i) => {
                           var parameter = _parameterDeclarations[i];
                           return p.Name.Equals(parameter.Name) &&
                                  p.GetActualType().Equals(parameter.GetActualType());
                       });
        }

        private void VerifyUniqueness(Type baseType, List<Type> types) {
            var joined = _parameters.Join(_outParameters,
                                          i => i.Name,
                                          o => o.Name,
                                          (i, o) => new {
                                              Parameter = i,
                                              OutParameter = o
                                          })
                                    .Where(v => {
                                        return !v.Parameter.IsByRef;
                                    })
                                    .ToDictionary(v => v.OutParameter.Name,
                                                  v => v.OutParameter);

            if (!joined.IsNullOrEmpty()) {
                Parameter parameter;
                var joinedTypes = _parameters.Select(p => {
                    if (joined.TryGetValue(p.Name, out parameter)) {
                        return parameter.Type;
                    }

                    return p.Type;
                });

                var ctor = baseType.GetConstructor(ReflectionUtils.AllFlags, Type.DefaultBinder, joinedTypes.ToArray(), null);

                if (ctor != null) {
                    var unquifierType = typeof(Uniquifier);
                    
                    types.Add(unquifierType);
                    _outParameters.Add(new Parameter("Uniquifier", unquifierType, -1));
                }
            }
        }
    }
}
