# ForbiddenChars

[English](README.md) | 日本語

string のフィールドから、禁止文字を含む値を代入できないプロパティを生成する C# の Source Generator です。
手入力する ID で、見分けにくい `0` と `O`、`1` と `l` を使わせたくないときに使います。

```csharp
using ForbiddenChars;

public partial class Voucher
{
    [ForbiddenChars("0O1l")]
    private string? _code;
}
```

`Code` プロパティが生成されます。

```csharp
var voucher = new Voucher { Code = "XYZ-4567" };   // OK
voucher.Code = "XYZ-0567";                          // ArgumentException
```

## 導入

```
dotnet add package ForbiddenChars
```

パッケージに入っているのは Generator だけで、実行時のアセンブリには何も追加されません。
Roslyn 4.8 以降 (.NET 8 SDK、Visual Studio 2022 17.8 以降) と C# 8 以降が必要です。
.NET Framework や .NET Standard 向けのプロジェクトでも使えますが、既定の言語バージョンが C# 7.3 なので `<LangVersion>8</LangVersion>` 以上を指定してください。

## 使い方

- `partial` なクラス・構造体・レコードの `string` フィールドに `[ForbiddenChars("...")]` を付けます。
- プロパティ名はフィールド名から決まります。`_code`、`m_code`、`code` はどれも `Code` になります。
- `null` は代入できます。
- `IgnoreCase = true` を指定すると、大文字と小文字を区別せずに判定します。

ネストした型やジェネリック型でも、外側の型がすべて `partial` なら使えます。

## 診断

| ID | 重大度 | 条件 |
|---|---|---|
| FC0001 | Error | フィールドを持つ型、またはその外側の型が `partial` でない |
| FC0002 | Error | フィールドが `string` でない |
| FC0003 | Error | 禁止文字が空 |

## 制限

- 文字は UTF-16 のコード単位ごとに比較するので、絵文字などのサロゲートペアの文字を個別に禁止することはできません。
- 検査はプロパティへの代入時に行います。フィールドへの直接代入や、禁止文字を含むリテラルはコンパイル時には検出しません。

紹介記事: [Source Generator で、手入力する ID に 0 と O を使えないようにする](https://leokun0210.hatenablog.com/entry/2026/09/21/Source_Generator_%E3%81%A7%E3%80%81%E6%89%8B%E5%85%A5%E5%8A%9B%E3%81%99%E3%82%8B_ID_%E3%81%AB_0_%E3%81%A8_O_%E3%82%92%E4%BD%BF%E3%81%88%E3%81%AA%E3%81%84%E3%82%88%E3%81%86%E3%81%AB%E3%81%99%E3%82%8B)
