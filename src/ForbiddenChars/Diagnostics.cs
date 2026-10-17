using Microsoft.CodeAnalysis;

namespace ForbiddenChars;

static class Diagnostics
{
	const string Category = "ForbiddenChars";

	public static readonly DiagnosticDescriptor TypeMustBePartial = new(
		"FC0001",
		"The containing type must be partial",
		"Type '{0}' must be declared partial so that a property can be generated for field '{1}'",
		Category,
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	public static readonly DiagnosticDescriptor FieldMustBeString = new(
		"FC0002",
		"The field must be of type string",
		"[ForbiddenChars] can only be applied to string fields, but '{0}' is not a string",
		Category,
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	public static readonly DiagnosticDescriptor EmptyChars = new(
		"FC0003",
		"The set of forbidden characters must not be empty",
		"[ForbiddenChars] on field '{0}' does not specify any characters to forbid",
		Category,
		DiagnosticSeverity.Error,
		isEnabledByDefault: true);
}
