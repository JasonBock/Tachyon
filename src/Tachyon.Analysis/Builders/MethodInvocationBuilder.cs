using Microsoft.CodeAnalysis.Text;
using Rocks.Analysis.Extensions;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Text;
using Tachyon.Analysis.Models;

namespace Tachyon.Analysis.Builders;

internal static class MethodInvocationBuilder
{
	internal static SourceText Build(ImmutableArray<MethodInvocationModel?> models)
	{
		using var writer = new StringWriter();
		using var indentWriter = new IndentedTextWriter(writer, "\t");

		indentWriter.WriteLines(
			"""
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

			""");

		// For each group, we emit the namespace of the containing type.
		// If it's null, then we don't emit a namespace
		var namespaceGroups = models.GroupBy(model => model!.ContainingTypeNamespace);

		foreach (var namespaceGroup in namespaceGroups)
		{
			var methodNamespace = namespaceGroup.Key;

			if (methodNamespace is not null)
			{
				indentWriter.WriteLines(
					$$"""
					namespace {{methodNamespace}}
					{
					""");
				indentWriter.Indent++;
			}

			var typeGroups = namespaceGroup.GroupBy(model => model!.ContainingTypeName);

			foreach (var typeGroup in typeGroups)
			{
				// global::System
				// TODO: How does interception work with generic type parameters?
				// Either from the type or the method itself.

				indentWriter.WriteLines(
					$$"""
					static file class {{typeGroup.Key}}Interceptors
					{
					""");
				indentWriter.Indent++;

				var methodGroups = typeGroup.GroupBy(model => model!.ToString());

				foreach (var methodGroup in methodGroups)
				{
					MethodInvocationModel? methodInformation = null;

					// We need to capture each intercept location,
					// but only use the first method for code generation.
					foreach (var method in methodGroup)
					{
						methodInformation = method!;
						// TODO: Maybe we can offer configuration to not emit
						// interception location
						indentWriter.WriteLine(
							$"""	
							[global::System.Runtime.CompilerServices.InterceptsLocation({method!.Location.Version}, "{method!.Location.Data}")] // {method!.Location.GetDisplayLocation()})
							""");
					}

					var parameters = string.Join(", ",
						methodInformation!.Parameters.Select(parameter => $"{parameter.TypeName} {parameter.Name}"));

					if (!methodInformation.IsStatic)
					{
						parameters = string.Join(", ",
							$"this {methodInformation.ContainingTypeName} @self", parameters);
					}
					
					indentWriter.WriteLines(
						$$"""
						public static {{methodInformation.ReturnTypeName}} {{methodInformation.Name}}({{parameters}})
						{
						""");

					indentWriter.Indent++;

					// TODO: Log the call.

					indentWriter.Indent--;
					indentWriter.WriteLine("}");
				}

				indentWriter.Indent--;
				indentWriter.WriteLine("}");
			}

			if (methodNamespace is not null)
			{
				indentWriter.Indent--;
				indentWriter.WriteLine("}");
			}
		}

		return SourceText.From(writer.ToString(), Encoding.UTF8);
	}
}