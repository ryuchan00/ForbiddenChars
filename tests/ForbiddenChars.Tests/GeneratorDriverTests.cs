using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ForbiddenChars.Tests;

public class GeneratorDriverTests
{
	static (ImmutableArray<Diagnostic> Generator, ImmutableArray<Diagnostic> Compilation) Run(string source)
	{
		var compilation = CSharpCompilation.Create(
			"Test",
			[CSharpSyntaxTree.ParseText(source)],
			[MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
			 MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location)!, "System.Runtime.dll"))],
			new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

		CSharpGeneratorDriver.Create(new ForbiddenCharsGenerator())
			.RunGeneratorsAndUpdateCompilation(compilation, out var output, out var diagnostics);

		return (diagnostics, output.GetDiagnostics());
	}

	[Fact]
	public void ReportsNonPartialType()
	{
		var (diagnostics, _) = Run("""
			using ForbiddenChars;
			class User { [ForbiddenChars("0")] string? _id; }
			""");

		var d = Assert.Single(diagnostics);
		Assert.Equal("FC0001", d.Id);
		Assert.Contains("'User'", d.GetMessage());
	}

	[Fact]
	public void ReportsNonPartialContainingType()
	{
		var (diagnostics, _) = Run("""
			using ForbiddenChars;
			class Outer { partial class User { [ForbiddenChars("0")] string? _id; } }
			""");

		var d = Assert.Single(diagnostics);
		Assert.Equal("FC0001", d.Id);
		Assert.Contains("'Outer'", d.GetMessage());
	}

	[Fact]
	public void ReportsNonStringField()
	{
		var (diagnostics, _) = Run("""
			using ForbiddenChars;
			partial class User { [ForbiddenChars("0")] int _id; }
			""");

		Assert.Equal("FC0002", Assert.Single(diagnostics).Id);
	}

	[Fact]
	public void ReportsEmptyChars()
	{
		var (diagnostics, _) = Run("""
			using ForbiddenChars;
			partial class User { [ForbiddenChars("")] string? _id; }
			""");

		Assert.Equal("FC0003", Assert.Single(diagnostics).Id);
	}

	[Fact]
	public void GeneratedCodeCompilesWithoutWarnings()
	{
		var (diagnostics, compilation) = Run("""
			using ForbiddenChars;
			namespace App;
			public partial class User
			{
				[ForbiddenChars("0O<&>")] string? _id;
				[ForbiddenChars("x", IgnoreCase = true)] string? _name;
			}
			""");

		Assert.Empty(diagnostics);
		Assert.Empty(compilation.Where(d => d.Severity >= DiagnosticSeverity.Warning && d.Id != "CS0169"));
	}
}
