namespace Tachyon.Analysis.Tests;

public sealed class MethodInvocationInterceptorGeneratorTests
{
	[Test]
	public async Task GenerateInstanceCallWithNoParametersAndVoidReturnAsync()
	{
		var code =
			"""
			namespace MyNamespace;

			public sealed class MyType
			{
				public void MyMethod() { }
			}

			public static class Invoker
			{
				public static void Invoke() => new MyType().MyMethod();
			}
			""";

		var interceptionCode =
			""""
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
			
			namespace MyNamespace
			{
				static file class MyTypeInterceptors
				{
					[global::System.Runtime.CompilerServices.InterceptsLocation(1, "ZT1wZ2W5wb/DuvmzRYWG2KgAAABUZXN0MC5jcw==")] // /0/Test0.cs(10,46))
					public static void MyMethod(this global::MyNamespace.MyType @self)
					{
						global::Tachyon.TachyonContext.Logger?.LogInformation(
							"""
							Method Invocation:
								Type: global::MyNamespace.MyType
								Is Instance: True
								Method Name: MyMethod
								Parameters: 
							""");
			
						@self.MyMethod();
					}
				}
			}
			
			"""";

		await TestAssistants.RunGeneratorAsync<MethodInvocationInterceptorGenerator>(
			code,
			[
				("MethodInterceptors.g.cs", interceptionCode)
			],
			["MyNamespace"]);
	}

	[Test]
	public async Task GenerateInstanceCallWithNoParametersAndReturnAsync()
	{
		var code =
			"""
			namespace MyNamespace;

			public sealed class MyType
			{
				public int MyMethod() => 2;
			}

			public static class Invoker
			{
				public static int Invoke() => new MyType().MyMethod();
			}
			""";

		var interceptionCode =
			""""
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
			
			namespace MyNamespace
			{
				static file class MyTypeInterceptors
				{
					[global::System.Runtime.CompilerServices.InterceptsLocation(1, "yvB4UkTgYlzf6M8RRwFh+agAAABUZXN0MC5jcw==")] // /0/Test0.cs(10,45))
					public static int MyMethod(this global::MyNamespace.MyType @self)
					{
						using var scope = global::Tachyon.TachyonContext.Logger?.BeginScope(global::System.Guid.NewGuid());
						
						global::Tachyon.TachyonContext.Logger?.LogInformation(
							"""
							Method Invocation:
								Type: global::MyNamespace.MyType
								Is Instance: True
								Method Name: MyMethod
								Parameters: 
							""");
			
						var @returnValue = @self.MyMethod();
						
						global::Tachyon.TachyonContext.Logger?.LogInformation(
							"""
							Method Invocation:
								Return Value: {ReturnValue}
							""", @returnValue);
						
						return @returnValue;
					}
				}
			}
			
			"""";

		await TestAssistants.RunGeneratorAsync<MethodInvocationInterceptorGenerator>(
			code,
			[
				("MethodInterceptors.g.cs", interceptionCode)
			],
			["MyNamespace"]);
	}

	[Test]
	public async Task GenerateInstanceCallWithParametersAndVoidReturnAsync()
	{
		var code =
			"""
			namespace MyNamespace;

			public sealed class MyType
			{
				public void MyMethod(int value) { }
			}

			public static class Invoker
			{
				public static void Invoke() => new MyType().MyMethod(2);
			}
			""";

		var interceptionCode =
			""""
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
			
			namespace MyNamespace
			{
				static file class MyTypeInterceptors
				{
					[global::System.Runtime.CompilerServices.InterceptsLocation(1, "MwO6ZXdCmiN1Yvl9/oTM07EAAABUZXN0MC5jcw==")] // /0/Test0.cs(10,46))
					public static void MyMethod(this global::MyNamespace.MyType @self, int value)
					{
						global::Tachyon.TachyonContext.Logger?.LogInformation(
							"""
							Method Invocation:
								Type: global::MyNamespace.MyType
								Is Instance: True
								Method Name: MyMethod
								Parameters: {value}
							""", value);
			
						@self.MyMethod(value);
					}
				}
			}
			
			"""";

		await TestAssistants.RunGeneratorAsync<MethodInvocationInterceptorGenerator>(
			code,
			[
				("MethodInterceptors.g.cs", interceptionCode)
			],
			["MyNamespace"]);
	}

	[Test]
	public async Task GenerateInstanceCallWithParametersAndReturnAsync()
	{
		var code =
			"""
			namespace MyNamespace;

			public sealed class MyType
			{
				public int MyMethod(int value) => 2;
			}

			public static class Invoker
			{
				public static int Invoke() => new MyType().MyMethod(2);
			}
			""";

		var interceptionCode =
			""""
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
			
			namespace MyNamespace
			{
				static file class MyTypeInterceptors
				{
					[global::System.Runtime.CompilerServices.InterceptsLocation(1, "IczUqt32dmYkkbaeWgzvIrEAAABUZXN0MC5jcw==")] // /0/Test0.cs(10,45))
					public static int MyMethod(this global::MyNamespace.MyType @self, int value)
					{
						using var scope = global::Tachyon.TachyonContext.Logger?.BeginScope(global::System.Guid.NewGuid());
						
						global::Tachyon.TachyonContext.Logger?.LogInformation(
							"""
							Method Invocation:
								Type: global::MyNamespace.MyType
								Is Instance: True
								Method Name: MyMethod
								Parameters: {value}
							""", value);
			
						var @returnValue = @self.MyMethod(value);
						
						global::Tachyon.TachyonContext.Logger?.LogInformation(
							"""
							Method Invocation:
								Return Value: {ReturnValue}
							""", @returnValue);
						
						return @returnValue;
					}
				}
			}
			
			"""";

		await TestAssistants.RunGeneratorAsync<MethodInvocationInterceptorGenerator>(
			code,
			[
				("MethodInterceptors.g.cs", interceptionCode)
			],
			["MyNamespace"]);
	}

	[Test]
	public async Task GenerateStaticCallWithNoParametersAndVoidReturnAsync()
	{
		var code =
			"""
			namespace MyNamespace;

			public static class MyType
			{
				public static void MyMethod() { }
			}

			public static class Invoker
			{
				public static void Invoke() => MyType.MyMethod();
			}
			""";

		var interceptionCode =
			""""
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
			
			namespace MyNamespace
			{
				static file class MyTypeInterceptors
				{
					[global::System.Runtime.CompilerServices.InterceptsLocation(1, "R2yjYigitV61GUMmRvKWU6kAAABUZXN0MC5jcw==")] // /0/Test0.cs(10,40))
					public static void MyMethod()
					{
						global::Tachyon.TachyonContext.Logger?.LogInformation(
							"""
							Method Invocation:
								Type: global::MyNamespace.MyType
								Is Instance: False
								Method Name: MyMethod
								Parameters: 
							""");
			
						global::MyNamespace.MyType.MyMethod();
					}
				}
			}
			
			"""";

		await TestAssistants.RunGeneratorAsync<MethodInvocationInterceptorGenerator>(
			code,
			[
				("MethodInterceptors.g.cs", interceptionCode)
			],
			["MyNamespace"]);
	}

	[Test]
	public async Task GenerateStaticCallWithNoParametersAndReturnAsync()
	{
		var code =
			"""
			namespace MyNamespace;

			public static class MyType
			{
				public static int MyMethod() => 2;
			}

			public static class Invoker
			{
				public static int Invoke() => MyType.MyMethod();
			}
			""";

		var interceptionCode =
			""""
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
			
			namespace MyNamespace
			{
				static file class MyTypeInterceptors
				{
					[global::System.Runtime.CompilerServices.InterceptsLocation(1, "TTADT4nGuS15Z6eDykbQ1qkAAABUZXN0MC5jcw==")] // /0/Test0.cs(10,39))
					public static int MyMethod()
					{
						using var scope = global::Tachyon.TachyonContext.Logger?.BeginScope(global::System.Guid.NewGuid());
						
						global::Tachyon.TachyonContext.Logger?.LogInformation(
							"""
							Method Invocation:
								Type: global::MyNamespace.MyType
								Is Instance: False
								Method Name: MyMethod
								Parameters: 
							""");
			
						var @returnValue = global::MyNamespace.MyType.MyMethod();
						
						global::Tachyon.TachyonContext.Logger?.LogInformation(
							"""
							Method Invocation:
								Return Value: {ReturnValue}
							""", @returnValue);
						
						return @returnValue;
					}
				}
			}
			
			"""";

		await TestAssistants.RunGeneratorAsync<MethodInvocationInterceptorGenerator>(
			code,
			[
				("MethodInterceptors.g.cs", interceptionCode)
			],
			["MyNamespace"]);
	}

	[Test]
	public async Task GenerateStaticCallWithParametersAndVoidReturnAsync()
	{
		var code =
			"""
			namespace MyNamespace;

			public static class MyType
			{
				public static void MyMethod(int value) { }
			}

			public static class Invoker
			{
				public static void Invoke() => MyType.MyMethod(2);
			}
			""";

		var interceptionCode =
			""""
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
			
			namespace MyNamespace
			{
				static file class MyTypeInterceptors
				{
					[global::System.Runtime.CompilerServices.InterceptsLocation(1, "9kiI/EsYjZH9gziVm7IuNbIAAABUZXN0MC5jcw==")] // /0/Test0.cs(10,40))
					public static void MyMethod(int value)
					{
						global::Tachyon.TachyonContext.Logger?.LogInformation(
							"""
							Method Invocation:
								Type: global::MyNamespace.MyType
								Is Instance: False
								Method Name: MyMethod
								Parameters: {value}
							""", value);
			
						global::MyNamespace.MyType.MyMethod(value);
					}
				}
			}
			
			"""";

		await TestAssistants.RunGeneratorAsync<MethodInvocationInterceptorGenerator>(
			code,
			[
				("MethodInterceptors.g.cs", interceptionCode)
			],
			["MyNamespace"]);
	}

	[Test]
	public async Task GenerateStaticCallWithParametersAndReturnAsync()
	{
		var code =
			"""
			namespace MyNamespace;

			public static class MyType
			{
				public static int MyMethod(int value) => 2;
			}

			public static class Invoker
			{
				public static int Invoke() => MyType.MyMethod(2);
			}
			""";

		var interceptionCode =
			""""
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
			
			namespace MyNamespace
			{
				static file class MyTypeInterceptors
				{
					[global::System.Runtime.CompilerServices.InterceptsLocation(1, "XqrWiSdsdyLyu032DD+Q9rIAAABUZXN0MC5jcw==")] // /0/Test0.cs(10,39))
					public static int MyMethod(int value)
					{
						using var scope = global::Tachyon.TachyonContext.Logger?.BeginScope(global::System.Guid.NewGuid());
						
						global::Tachyon.TachyonContext.Logger?.LogInformation(
							"""
							Method Invocation:
								Type: global::MyNamespace.MyType
								Is Instance: False
								Method Name: MyMethod
								Parameters: {value}
							""", value);
			
						var @returnValue = global::MyNamespace.MyType.MyMethod(value);
						
						global::Tachyon.TachyonContext.Logger?.LogInformation(
							"""
							Method Invocation:
								Return Value: {ReturnValue}
							""", @returnValue);
						
						return @returnValue;
					}
				}
			}
			
			"""";

		await TestAssistants.RunGeneratorAsync<MethodInvocationInterceptorGenerator>(
			code,
			[
				("MethodInterceptors.g.cs", interceptionCode)
			],
			["MyNamespace"]);
	}

	[Test]
	public async Task GenerateWhenTargetTypeOfInvocationIsNotInNamespaceAsync()
	{
		var code =
			"""
			namespace MyNamespace;

			public static class MyType
			{
				public static void MyMethod(int value) { }
			}

			public static class Invoker
			{
				public static void Invoke() => MyType.MyMethod(2);
			}
			""";

		var interceptionCode =
			""""
			#nullable enable
			
			using Microsoft.Extensions.Logging;
			
			namespace Tachyon
			{
				public static class TachyonContext
				{
					public static global::Microsoft.Extensions.Logging.ILogger? Logger { get; set; }
				}
			}

			"""";

		await TestAssistants.RunGeneratorAsync<MethodInvocationInterceptorGenerator>(
			code,
			[
				("MethodInterceptors.g.cs", interceptionCode)
			],
			["OtherNamespace"]);
	}

	[Test]
	public async Task GenerateWithTypeInParameterAsFullyQualifiedNameAsync()
	{
		var code =
			"""
			namespace MyNamespace;

			public sealed record MyCustomType(int Value);

			public static class MyType
			{
				public static MyCustomType MyMethod(MyCustomType value) => value;
			}

			public static class Invoker
			{
				public static MyCustomType Invoke() => MyType.MyMethod(new MyCustomType(2));
			}
			""";

		var interceptionCode =
			""""
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
			
			namespace MyNamespace
			{
				static file class MyTypeInterceptors
				{
					[global::System.Runtime.CompilerServices.InterceptsLocation(1, "iaYfA9qe9zj01zBG4rlY7QIBAABUZXN0MC5jcw==")] // /0/Test0.cs(12,48))
					public static global::MyNamespace.MyCustomType MyMethod(global::MyNamespace.MyCustomType value)
					{
						using var scope = global::Tachyon.TachyonContext.Logger?.BeginScope(global::System.Guid.NewGuid());
						
						global::Tachyon.TachyonContext.Logger?.LogInformation(
							"""
							Method Invocation:
								Type: global::MyNamespace.MyType
								Is Instance: False
								Method Name: MyMethod
								Parameters: {value}
							""", value);
			
						var @returnValue = global::MyNamespace.MyType.MyMethod(value);
						
						global::Tachyon.TachyonContext.Logger?.LogInformation(
							"""
							Method Invocation:
								Return Value: {ReturnValue}
							""", @returnValue);
						
						return @returnValue;
					}
				}
			}
			
			"""";

		await TestAssistants.RunGeneratorAsync<MethodInvocationInterceptorGenerator>(
			code,
			[
				("MethodInterceptors.g.cs", interceptionCode)
			],
			["MyNamespace"]);
	}

	[Test]
	public async Task GenerateWithMultipleNamespacesAsync()
	{
		var code =
			"""
			using AnotherNamespace;
			using MyNamespace;

			namespace AnotherNamespace
			{
				public static class AnotherType
				{
					public static void AnotherMethod() { }
				}
			}
			
			namespace MyNamespace
			{
				public static class MyType
				{
					public static void MyMethod() { }
				}
			}


			public static class Invoker
			{
				public static void Invoke()
				{
					AnotherType.AnotherMethod();
					MyType.MyMethod();
				}
			}
			""";

		var interceptionCode =
			""""
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
			
			namespace AnotherNamespace
			{
				static file class AnotherTypeInterceptors
				{
					[global::System.Runtime.CompilerServices.InterceptsLocation(1, "YAbi4XgwmSHmrWlMZNyzL2IBAABUZXN0MC5jcw==")] // /0/Test0.cs(25,15))
					public static void AnotherMethod()
					{
						global::Tachyon.TachyonContext.Logger?.LogInformation(
							"""
							Method Invocation:
								Type: global::AnotherNamespace.AnotherType
								Is Instance: False
								Method Name: AnotherMethod
								Parameters: 
							""");
			
						global::AnotherNamespace.AnotherType.AnotherMethod();
					}
				}
			}
			namespace MyNamespace
			{
				static file class MyTypeInterceptors
				{
					[global::System.Runtime.CompilerServices.InterceptsLocation(1, "YAbi4XgwmSHmrWlMZNyzL30BAABUZXN0MC5jcw==")] // /0/Test0.cs(26,10))
					public static void MyMethod()
					{
						global::Tachyon.TachyonContext.Logger?.LogInformation(
							"""
							Method Invocation:
								Type: global::MyNamespace.MyType
								Is Instance: False
								Method Name: MyMethod
								Parameters: 
							""");
			
						global::MyNamespace.MyType.MyMethod();
					}
				}
			}
			
			"""";

		await TestAssistants.RunGeneratorAsync<MethodInvocationInterceptorGenerator>(
			code,
			[
				("MethodInterceptors.g.cs", interceptionCode)
			],
			["AnotherNamespace", "MyNamespace"]);
	}
}