using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using Tachyon.Analysis.Builders;
using Tachyon.Analysis.Extensions;
using Tachyon.Analysis.Models;

namespace Tachyon.Analysis;

[Generator]
internal sealed class MethodInvocationInterceptorGenerator
	: IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		// TODO: Make sure we only get one. If there are none,
		// that's kind of an issue.

		// TODO: For the POC,
		// it's expected that the text file will have a format like this:
		/*
		global::System.Guid.NewGuid
		global::MyLibrary.MyNamespace
		global::MyLibrary
		*/
		// I'll probably need to make this JSON or XML to potentially provide
		// more configuration options.
		var configurationFiles = context.AdditionalTextsProvider
			.Where(_ => _.Path.EndsWith("tachyon.txt"))
			.Select((text, token) =>
			{
				var filters = new HashSet<string>();
				var sourceText = text.GetText(token);

				if (sourceText is not null)
				{
					foreach (var lineText in from line in sourceText.Lines
													 where line.Text is not null
													 let lineText = line.Text!.ToString()
													 where !string.IsNullOrWhiteSpace(lineText)
													 select lineText)
					{
						_ = filters.Add(lineText);
					}
				}

				return new EquatableArray<string>([.. filters]);
			})
			.Collect();

		var models = context.SyntaxProvider.CreateSyntaxProvider(
			(node, token) => node is InvocationExpressionSyntax,
			(context, token) =>
			{
				var invocationNode = (InvocationExpressionSyntax)context.Node;
				var invocationSymbol = context.SemanticModel.GetSymbolInfo(invocationNode, token).Symbol as IMethodSymbol;

				if (invocationSymbol is not null && !invocationSymbol.IsPartialDefinition &&
					context.SemanticModel.GetInterceptableLocation(invocationNode, token) is { } location)
				{
					var parameters = invocationSymbol.Parameters
						.Select(parameter => new ParameterModel(
							parameter.Name,
							parameter.Type.GetFullyQualifiedName(context.SemanticModel.Compilation)))
						.ToImmutableArray();

					var @namespace = invocationSymbol.ContainingType.ContainingNamespace is not null ?
						!invocationSymbol.ContainingType.ContainingNamespace.IsGlobalNamespace ?
							invocationSymbol.ContainingType.ContainingNamespace.ToDisplayString() :
							null :
						null;

					// TODO: I believe it's OK to capture InterceptableLocation
					// in your own model, but need to verify that.

					// TODO: The signature...maybe we could use the hash code of the display string
					// to save a bit on space.

					return new MethodInvocationModel(
						invocationSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
						invocationSymbol.Name,
						invocationSymbol.ContainingType.Name,
						@namespace,
						invocationSymbol.IsStatic,
						parameters,
						invocationSymbol.ReturnType.GetFullyQualifiedName(context.SemanticModel.Compilation),
						location);
				}

				return null;
			})
			.Where(model => model is not null)
			//.Combine(configurationFiles)
			//.Where(provider =>
			//	provider.Right.Length > 0 &&
			//	provider.Right[0].Any(filter => $"{provider.Left!.ContainingTypeName}.{provider.Left.Name}".StartsWith(filter)))
			//.Select((provider, token) => provider.Left)
			.Collect();

		context.RegisterSourceOutput(models, MethodInvocationInterceptorGenerator.CreateOutput);
	}

	private static void CreateOutput(SourceProductionContext context, ImmutableArray<MethodInvocationModel?> source)
	{
		if (source.Length > 0)
		{
			var methodGroups = source.GroupBy(model => model!.ContainingTypeNamespace);

			var methodText = MethodInvocationBuilder.Build(methodGroups);

			context.AddSource("MethodInterceptors.g.cs", methodText);
		}
	}
}