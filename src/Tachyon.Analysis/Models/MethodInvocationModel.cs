namespace Tachyon.Analysis.Models;

internal sealed record MethodInvocationModel(
	string Name, string ContainingTypeName, EquatableArray<ParameterModel> Parameters, string ReturnTypeName);