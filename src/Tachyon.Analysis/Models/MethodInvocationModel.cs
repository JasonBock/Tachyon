using Microsoft.CodeAnalysis.CSharp;

namespace Tachyon.Analysis.Models;

internal sealed record MethodInvocationModel(
	string Name, 
	string ContainingTypeName,
	string FullyQualifiedContainingTypeName,
	string? ContainingTypeNamespace,
	bool IsStatic,
	EquatableArray<ParameterModel> Parameters, 
	bool HasReturnValue,
	string FullyQualifiedReturnTypeName,
	InterceptableLocation Location)
{
	public override string ToString() => $"{this.FullyQualifiedReturnTypeName} {this.ContainingTypeName}.{this.Name}({string.Join(", ", this.Parameters.Select(parameter => parameter.TypeName))})";
}