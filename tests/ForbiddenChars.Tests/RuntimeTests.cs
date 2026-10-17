using ForbiddenChars;

namespace ForbiddenChars.Tests
{
	public partial class User
	{
		[ForbiddenChars("0O1l")]
		string? _manualId;

		[ForbiddenChars("o", IgnoreCase = true)]
		string? m_displayId;

		[ForbiddenChars("{\"}")]
		string? tricky;
	}

	public partial class Outer<T>
	{
		public partial record struct Inner
		{
			[ForbiddenChars("-")]
			string? _code;
		}
	}

	public class RuntimeTests
	{
		[Fact]
		public void AcceptsValueWithoutForbiddenCharacters()
		{
			var user = new User { ManualId = "XYZ-4567" };

			Assert.Equal("XYZ-4567", user.ManualId);
		}

		[Theory]
		[InlineData("XYZ-0567", '0', 4)]
		[InlineData("XYZ-O567", 'O', 4)]
		[InlineData("lXYZ", 'l', 0)]
		public void RejectsValueWithForbiddenCharacter(string value, char offending, int index)
		{
			var user = new User();

			var ex = Assert.Throws<ArgumentException>(() => user.ManualId = value);

			Assert.Equal("value", ex.ParamName);
			Assert.StartsWith($"'{offending}' is not allowed (index {index}, value \"{value}\"). Forbidden characters: 0O1l", ex.Message);
			Assert.Null(user.ManualId);
		}

		[Fact]
		public void AcceptsNull()
		{
			var user = new User { ManualId = "ABC" };

			user.ManualId = null;

			Assert.Null(user.ManualId);
		}

		[Fact]
		public void IgnoreCaseRejectsBothCases()
		{
			var user = new User();

			Assert.Throws<ArgumentException>(() => user.DisplayId = "BOB");
			Assert.Throws<ArgumentException>(() => user.DisplayId = "bob");
			user.DisplayId = "abc";
			Assert.Equal("abc", user.DisplayId);
		}

		[Fact]
		public void CaseSensitiveByDefault()
		{
			var user = new User { ManualId = "o" };

			Assert.Equal("o", user.ManualId);
		}

		[Theory]
		[InlineData("{")]
		[InlineData("\"")]
		[InlineData("}")]
		public void HandlesCharactersThatNeedEscaping(string value)
		{
			var user = new User();

			Assert.Throws<ArgumentException>(() => user.Tricky = value);
		}

		[Fact]
		public void WorksInNestedGenericTypes()
		{
			var inner = new Outer<int>.Inner { Code = "ABC" };

			Assert.Equal("ABC", inner.Code);
			Assert.Throws<ArgumentException>(() => inner.Code = "A-B");
		}
	}
}
