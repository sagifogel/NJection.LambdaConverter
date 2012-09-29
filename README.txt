NJection.LambdaConverter is an open-source .NET assembly for converting delegates resolved from methods/constructors to expression trees.
Website: http://www.njection.net/

Copyright 2009-2012 Sagi Fogel
License: NJection.LambdaConverter is distributed under the MIT License.

Included open-source libraries:
		Mono.Cecil
		ICSharpCode.Decompiler
		ICSharpCode.NRefactory
		ICSharpCode.NRefactory.CSharp

Samples:

		Converting a static delegate

		public class Program
		{
			  private static void Main(string[] args) {
				   var lambda = Lambda.TransformMethodTo<Func<string, int>>()
									  .From(() => Parse)
									  .ToLambda();            
			  }

			  public static int Parse(string value) {
				   return int.Parse(value);
			  }
		}

		-------------------------------------------------------------------------
		Converting an instance delegate

		public class Sample
		{
			  public int Parse(string value) {
				  return int.Parse(value);
			  }
		}

		public class Program
		{
			  private static void Main(string[] args) {
				   var sample = new Sample();
				   var lambda = Lambda.TransformMethodTo<Func<string, int>>()
									  .From(() => sample.Parse)
									  .WithContextOf<Sample>(sample)
									  .ToLambda();              
			  }
		}

		-------------------------------------------------------------------------
		Converting a constructor

		public class Sample
		{
			 public int Value { get; set; }

			 public Sample(int i) {
				  Value = i;
			 }
		}

		public class Program
		{
			  private static void Main(string[] args) {
				   var ctor = typeof(Sample).GetConstructor(new Type[] { typeof(int) });
				   var resolver = new ConstructorResolver(ctor);
				   var lambda2 = Lambda.ResolveConstructorTo<Func<int, Sample>>(resolver)
									   .ToLambda();            
			  }
		}

		-------------------------------------------------------------------------
		Converting a generic delegate

		public class Program
		{
			  private static void Main(string[] args) {
				  var lambda = Lambda.TransformMethodTo<Func<string, DateTime>>()
									 .From(() => Parse<DateTime>)
									 .ToLambda();           
			  }

			  static T Parse<T>(string value) where T : struct {
				   return (T)Enum.Parse(typeof(T), value);
			  }
		}