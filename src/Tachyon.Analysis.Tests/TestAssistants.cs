using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using NuGet.Frameworks;

namespace Tachyon.Analysis.Tests;

internal static class TestAssistants
{
	internal static async Task RunGeneratorAsync<TGenerator>(string code,
		IEnumerable<(string, string)> generatedSources,
		SourceText additionalFileContent,
		IEnumerable<MetadataReference>? additionalReferences = null)
		where TGenerator : IIncrementalGenerator, new()
	{
		var test = new IncrementalGeneratorTest<TGenerator>(ReportDiagnostic.Default)
		{
			ReferenceAssemblies = TestAssistants.GetNet10(),
			TestState =
			{
				Sources = { code },
				OutputKind = OutputKind.DynamicallyLinkedLibrary,
			},
		};

		test.TestState.AdditionalFiles.Add(("tachyon.txt", additionalFileContent));

		foreach (var (generatedFileName, generatedCode) in generatedSources)
		{
			test.TestState.GeneratedSources.Add((typeof(TGenerator), generatedFileName, generatedCode));
		}

		test.TestState.AdditionalReferences.Add(typeof(TGenerator).Assembly);

		if (additionalReferences is not null)
		{
			test.TestState.AdditionalReferences.AddRange(additionalReferences);
		}

		await test.RunAsync();
	}

	private static ReferenceAssemblies GetNet10()
	{
		// Always look here for the latest version of a particular runtime:
		// https://www.nuget.org/packages/Microsoft.NETCore.App.Ref
		if (!NuGetFramework.Parse("net10.0").IsPackageBased)
		{
			// The NuGet version provided at runtime does not recognize the 'net10.0' target framework
			throw new NotSupportedException("The 'net10.0' target framework is not supported by this version of NuGet.");
		}

		return new ReferenceAssemblies(
			 "net10.0",
			 new PackageIdentity(
				  "Microsoft.NETCore.App.Ref",
				  "10.0.0-rc.1.25451.107"),
			 Path.Combine("ref", "net10.0"));
	}
}