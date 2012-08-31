using System;
using System.Linq.Expressions;

namespace NJection.LambdaConverter.Fluent
{
    internal abstract class Expressionizer<TDelegate> : IMethodTranformer<TDelegate>, IMethodCompiler<TDelegate>, IContextProvider<TDelegate>
    {
        protected object Context { get; set; }
        protected Type DeclaringType { get; set; }
        protected Func<TDelegate> MethodResolver { get; set; }

        public virtual IContextProvider<TDelegate> From(Func<TDelegate> methodResolver) {
            MethodResolver = methodResolver;
            ResolveMethod();
            return this;
        }

        protected abstract void ResolveMethod();
        public abstract Expression<TDelegate> ToLambda();

        public virtual IContextProvider<TDelegate> WithContextOf<TContext>(TContext context) {
            Context = context;
            EnsureContextIsAssignanbleFromDeclaringType();
            return this;
        }

        protected virtual void EnsureContextIsAssignanbleFromDeclaringType() {
            if (Context != null) {
                Type contextType = Context.GetType();

                if (!DeclaringType.IsAssignableFrom(contextType)) {
                    throw new MemberAccessException(string.Format("The type {0} is not assignable from type {1}", DeclaringType.Name, contextType));
                }
            }
        }

        public TDelegate Compile() {
            return ToLambda().Compile();
        }
    }
}