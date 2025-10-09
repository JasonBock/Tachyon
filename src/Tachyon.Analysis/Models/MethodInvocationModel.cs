using Microsoft.CodeAnalysis.CSharp;

namespace Tachyon.Analysis.Models;

internal sealed record MethodInvocationModel(
	string Name, string ContainingTypeName, string? ContainingTypeNamespace,
	bool IsStatic,
	EquatableArray<ParameterModel> Parameters, string ReturnTypeName,
	InterceptableLocation Location)
{
	public override string ToString() => $"{this.ReturnTypeName} {this.ContainingTypeName}.{this.Name}({string.Join(", ", this.Parameters.Select(parameter => parameter.TypeName))})";
}