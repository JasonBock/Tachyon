using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
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
						filters.Add(lineText);
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

				if (invocationSymbol is not null && !invocationSymbol.IsPartialDefinition)
				{
					var parameters = invocationSymbol.Parameters
						.Select(parameter => new ParameterModel(
							parameter.Name,
							parameter.Type.GetFullyQualifiedName(context.SemanticModel.Compilation)))
						.ToImmutableArray();

					return new MethodInvocationModel(
						invocationSymbol.Name,
						invocationSymbol.ContainingType.GetFullyQualifiedName(context.SemanticModel.Compilation),
						parameters,
						invocationSymbol.ReturnType.GetFullyQualifiedName(context.SemanticModel.Compilation));
				}

				return null;
			})
			.Where(model => model is not null)
			.Combine(configurationFiles)
			.Where(provider => 
				provider.Right.Length > 0 && 
				provider.Right[0].Any(filter => $"{provider.Left!.ContainingTypeName}.{provider.Left.Name}".StartsWith(filter)))
			.Select((provider, token) => provider.Left)
			.Collect();

		// TODO: Register the output and start generating code!
	}
}

/*
global::System.Guid.NewGuid
global::MyLibrary.MyNamespace
global::MyLibrary
*/