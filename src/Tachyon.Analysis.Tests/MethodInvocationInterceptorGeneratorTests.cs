using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using System.Text;

namespace Tachyon.Analysis.Tests;

internal static class MethodInvocationInterceptorGeneratorTests
{
	[Test]
	public static async Task GenerateAbstractAsync()
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