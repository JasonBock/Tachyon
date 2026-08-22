To sketch things out, first, let's say this is our target:

```c#
namespace Tachyon.Example;

public static class Processor
{
	public static int Process(int value) => value * 2;
}
```

We have a program that invokes it:

```c#
using Microsoft.Extensions.Logging;
using Tachyon;
using Tachyon.Example;

using var factory = LoggerFactory.Create(builder => builder.AddConsole(options => options.IncludeScopes = true));
var logger = factory.CreateLogger("Program");
TachyonContext.Logger = logger;

logger.LogInformation("About to invoke...");

var result = Processor.Process(2);

logger.LogInformation("Invocation complete, result is {result}", result);
```

With Tachyon in play (assuming `<InterceptorsNamespaces>$(InterceptorsNamespaces);Tachyon.Example</InterceptorsNamespaces>` is set), we generate this code:

```c#
#nullable enable

using Microsoft.Extensions.Logging;

namespace Tachyon
{
	public static class TachyonContext
	{
		public static global::Microsoft.Extensions.Logging.ILogger? Logger { get; set; }
	}
}

namespace System.Runtime.CompilerServices
{
	[global::System.Diagnostics.Conditional("DEBUG")]
	[global::System.AttributeUsage(global::System.AttributeTargets.Method, AllowMultiple = true)]
	sealed file class InterceptsLocationAttribute 
		: global::System.Attribute
	{
		public InterceptsLocationAttribute(int version, string data)
		{
			_ = version;
			_ = data;
		}
	}
}

namespace Tachyon.Example
{
	static file class ProcessorInterceptors
	{
		[global::System.Runtime.CompilerServices.InterceptsLocation(1, "U//UdJD1/GkyjJfSAFIdghoCAABQcm9ncmFtLmNz")]
		public static int Process(int value)
		{
			using var scope = global::Tachyon.TachyonContext.Logger?.BeginScope(global::System.Guid.NewGuid());
			
			global::Tachyon.TachyonContext.Logger?.LogInformation(
				"""
				Method Invocation:
					Type: global::Tachyon.Example.Processor
					Is Instance: False
					Method Name: Process
					Parameters: {value}
				""", value);

			var @returnValue = global::Tachyon.Example.Processor.Process(value);

			global::Tachyon.TachyonContext.Logger?.LogInformation(
				"""
				Method Invocation:
					Return Value: {ReturnValue}
				""", @returnValue);
			
			return @returnValue;
		}
	}
}
```

What I'd like to do is this:

```c#
#nullable enable

using Microsoft.Extensions.Logging;

namespace Tachyon
{
	public static class TachyonContext
	{
		public static global::Microsoft.Extensions.Logging.ILogger? Logger { get; set; }
	}
}

namespace System.Runtime.CompilerServices
{
	[global::System.Diagnostics.Conditional("DEBUG")]
	[global::System.AttributeUsage(global::System.AttributeTargets.Method, AllowMultiple = true)]
	sealed file class InterceptsLocationAttribute 
		: global::System.Attribute
	{
		public InterceptsLocationAttribute(int version, string data)
		{
			_ = version;
			_ = data;
		}
	}
}

namespace Tachyon.Interceptors
{
    namespace Tachyon.Example
    {
        static file partial class ProcessorInterceptors
        {
            [global::System.Runtime.CompilerServices.InterceptsLocation(1, "U//UdJD1/GkyjJfSAFIdghoCAABQcm9ncmFtLmNz")]
            public static partial int Process(int value);
        }
    }
}
```

The point is, Tachyon no longer just logs, in fact, it wouldn't do anything other than creating a `partial` member. It's now up to the user to provide something like this:

```c#
namespace Tachyon.Interceptors.Tachyon.Example
{
	static file partial class ProcessorInterceptors
	{
		public static partial int Process(int value)
        {
            // Do whatever I want with this invocation.
        }
	}
}
```

This is nice because the compiler is going to enforce, but it's also a bit annoying because the user has to define this. Maybe a refactoring could make this easier.

We may want to keep the logging "aspect" around, generalize that concept and make it configurable, so other "aspects" can be defined. And also keep a concept where you could "aspects", but also force the use to provide an implementation, where an `On..` method is generated in the interceptor that's `partial`:

```c#
var @returnValue = global::Tachyon.Interceptors.Tachyon.Example.ProcessorInterceptors.OnProcess(value);
```

Random idea for filtering and "plugging aspects" in:

```xml
<Filters>
    <Filter Value="global::System.String.IsNullOr" />
    <Filter Value="global::System.Guid" GeneratePartial="true">
        <Aspects>
            <Logger Location="Pre">
            <Kafka Location="PreAndPost">
            <Messaging Location="Post">
        </Aspects>
    </Filter>
    <Filter Value="global::" />
</Filters>
```

TODO:
* Update NuGet packages
* Consider putting **all** gen-d code into either `TachyonContext` or `TachyonInterceptors` namespace, something like that.
* Create a refactoring to generate the interceptor class based on a invocation (see Rocks for an example of this)