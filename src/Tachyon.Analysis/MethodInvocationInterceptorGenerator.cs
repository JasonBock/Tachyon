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

					return new MethodInvocationModel(
						invocationSymbol.Name,
						invocationSymbol.ContainingType.Name,
						invocationSymbol.ContainingType.GetFullyQualifiedName(context.SemanticModel.Compilation),
						@namespace,
						invocationSymbol.IsStatic,
						parameters,
						!invocationSymbol.ReturnsVoid,
						invocationSymbol.ReturnType.GetFullyQualifiedName(context.SemanticModel.Compilation),
						location);
				}

				return null;
			})
			.Where(model => model is not null)
			.Combine(context.ParseOptionsProvider)
			.Where(provider =>
				provider.Right.Features.Count > 0 &&
				provider.Right.Features.Any(filter => 
					filter.Key == "InterceptorsNamespaces" &&
					filter.Value.Split(';').Contains(provider.Left!.ContainingTypeNamespace)))
			.Select((provider, token) => provider.Left)
			.Collect();

		context.RegisterSourceOutput(models, MethodInvocationInterceptorGenerator.CreateOutput);
	}

	private static void CreateOutput(SourceProductionContext context, ImmutableArray<MethodInvocationModel?> source)
	{
		if (source.Length > 0)
		{
			context.AddSource("MethodInterceptors.g.cs", MethodInvocationBuilder.Build(source));
		}
	}
}