# ForbiddenChars

[![CI](https://github.com/ryuchan00/ForbiddenChars/actions/workflows/ci.yml/badge.svg)](https://github.com/ryuchan00/ForbiddenChars/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/ForbiddenChars.svg)](https://www.nuget.org/packages/ForbiddenChars)

English | [日本語](README.ja.md)

A C# source generator that turns a string field into a property whose setter rejects
forbidden characters — for example the easily confused `0`/`O` and `1`/`l` in IDs that people
type by hand.

```csharp
using ForbiddenChars;

public partial class Voucher
{
    [ForbiddenChars("0O1l")]
    private string? _code;
}
```

The generator adds a `Code` property:

```csharp
var voucher = new Voucher { Code = "XYZ-4567" };   // OK
voucher.Code = "XYZ-0567";                          // throws ArgumentException
```

```
'0' is not allowed (index 4, value "XYZ-0567"). Forbidden characters: 0O1l (Parameter 'value')
```

## Why

When a rule like "no `0` or `O` in voucher codes" lives in a validator that callers have to
remember to invoke, someone eventually forgets. Putting the check in the setter makes it
impossible to store a bad value through the property, and generating that setter keeps the
rule down to one attribute.

## Installation

```
dotnet add package ForbiddenChars
```

The package contains only the generator; nothing is added to your output assembly at
runtime. It needs the Roslyn 4.8 compiler or later (.NET 8 SDK or Visual Studio 2022 17.8 and later) and
C# 8 or later. Projects targeting .NET Framework or .NET Standard work too, but they default to
C# 7.3, so set `<LangVersion>8</LangVersion>` or higher.

## Usage

- Put `[ForbiddenChars("...")]` on a `string` field of a `partial` class, struct or record.
- The property name comes from the field name: `_code`, `m_code` and `code` all become `Code`.
- `null` is accepted.
- `IgnoreCase = true` compares characters case-insensitively (ordinal):

```csharp
[ForbiddenChars("o", IgnoreCase = true)]   // rejects both "o" and "O"
private string? _displayId;
```

Nested and generic types work as long as every containing type is `partial`.

## Diagnostics

| ID | Severity | When |
|---|---|---|
| FC0001 | Error | The field's type, or a type containing it, is not `partial` |
| FC0002 | Error | The field is not a `string` |
| FC0003 | Error | The attribute lists no characters |

## Limitations

- Characters are compared one UTF-16 code unit at a time, so characters outside the Basic
  Multilingual Plane (such as most emoji) cannot be forbidden individually.
- The check runs when the property is set. Assigning the field directly, or writing a forbidden
  literal in source, is not reported at compile time.

## License

MIT
