using NUnit.Framework;

namespace Tachyon.Analysis.Tests;

internal static class MethodInvocationInterceptorGeneratorTests
{
	[Test]
	public static async Task GenerateWithParametersAndVoidReturnAsync()
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
						using var scope = global::Tachyon.TachyonContext.Logger?.BeginScope(global::System.Guid.NewGuid());
						
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
	public static async Task GenerateWhenTargetTypeOfInvocationIsNotInNamespaceAsync()
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
	public static async Task GenerateWithTypeInParameterAsFullyQualifiedNameAsync()
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
}