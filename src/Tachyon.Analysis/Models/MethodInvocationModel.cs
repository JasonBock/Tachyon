using Microsoft.CodeAnalysis.CSharp;

namespace Tachyon.Analysis.Models;

internal sealed record MethodInvocationModel(
	string Signature,
	string Name, string ContainingTypeName, string? ContainingTypeNamespace,
	bool IsStatic,
	EquatableArray<ParameterModel> Parameters, string ReturnTypeName,
	InterceptableLocation Location);