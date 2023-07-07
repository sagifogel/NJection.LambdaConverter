## NJection.LambdaConverter 

`NJection.LambdaConverter` is an open-source .NET assembly for converting delegates<br/> resolved from methods/constructors to expression trees.<br/>

### Installing

```
Install-Package NJection.LambdaConverter
```

### Converting a static delegate

```c#
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
```

### Converting an instance delegate

```c#
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
```

### Converting a constructor

```c#
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
	    var types = new Type[] { typeof(int) };
	    var ctor = typeof(Sample).GetConstructor(types);
	    var resolver = new ConstructorResolver(ctor);
	    var lambda = Lambda.ResolveConstructorTo<Func<int, Sample>>(resolver)
			       .ToLambda();            
	}
}
```

### Converting a generic delegate

```c#
public class Program
{
	private static void Main(string[] args) {
	     var lambda = Lambda.TransformMethodTo<Func<string, DateTime>>()
				.From(() => Parse<DateTime>)
				.ToLambda();           
	}

	private static T Parse<T>(string value) where T : struct {
	    return (T)Enum.Parse(typeof(T), value);
	}
}
```

### Converting a custom delegate

```c#
public class Program
{	
	private delegate int CustomDelegate(string value);

	private static void Main(string[] args) {
		var lambda = Lambda.TransformMethodTo<CustomDelegate>()
                                   .From(() => Parse)
                                   .ToLambda();           
	}

	private static int Parse(string value) {
		return int.Parse(value);
	}
}
```

### Included open-source libraries:
```
Mono.Cecil
ICSharpCode.Decompiler
ICSharpCode.NRefactory
ICSharpCode.NRefactory.CSharp
```

## License

`NJection.LambdaConverter` is distributed under the MIT License.<br/>
