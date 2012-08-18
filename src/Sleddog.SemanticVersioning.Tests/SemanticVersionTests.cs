using System;
using System.Linq;
using Ploeh.AutoFixture.Xunit;
using Sleddog.SemanticVersioning;
using Xunit;
using Xunit.Extensions;

namespace SemanticVersioning.Tests
{
	public class SemanticVersionTests
	{
		[Theory, AutoData]
		public void CanInstantiate(ushort major, ushort minor, ushort patch)
		{
			Assert.DoesNotThrow(() => new SemanticVersion(major, minor, patch));
		}

		[Theory]
		[InlineAutoData(SemanticVersionType.Build)]
		[InlineAutoData(SemanticVersionType.PreRelease)]
		public void CanInstantiateWithSpecialVersionWithNonNormalType(SemanticVersionType semanticVersionType, ushort major, ushort minor,
		                                                              ushort patch,
		                                                              string[] specialVersionParts)
		{
			Assert.DoesNotThrow(() => new SemanticVersion(major, minor, patch, specialVersionParts, semanticVersionType));
		}

		[Theory]
		[InlineAutoData(SemanticVersionType.Normal)]
		public void CanNotInstantiateWithSpecialVersionWithNormalType(SemanticVersionType semanticVersionType, ushort major, ushort minor,
		                                                              ushort patch,
		                                                              string[] specialVersionParts)
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new SemanticVersion(major, minor, patch, specialVersionParts, semanticVersionType));
		}

		[Theory]
		[InlineAutoData(SemanticVersionType.Build)]
		public void ThrowsExceptionOnMalformedSpecialVersionParts(SemanticVersionType semanticVersionType, ushort major, ushort minor,
		                                                          ushort patch,
		                                                          string[] specialVersionParts)
		{
			var malformedSpecialPart = ".,";

			var versionParts = specialVersionParts.Concat(new[] {malformedSpecialPart});

			Assert.Throws<ArgumentException>(() => new SemanticVersion(major, minor, patch, versionParts, semanticVersionType));
		}

		[Theory, AutoData]
		public void NormalTypeValuesGetsSetCorrectly(ushort major, ushort minor, ushort patch)
		{
			var sut = new SemanticVersion(major, minor, patch);

			Assert.Equal(major, sut.Major);
			Assert.Equal(minor, sut.Minor);
			Assert.Equal(patch, sut.Patch);
			Assert.Null(sut.SpecialVersion);
		}

		[Theory]
		[InlineAutoData(SemanticVersionType.Build)]
		[InlineAutoData(SemanticVersionType.PreRelease)]
		public void SpecialVersionsGetsSetCorrectly(SemanticVersionType semanticVersionType, ushort major, ushort minor, ushort patch,
		                                            string[] specialVersionParts)
		{
			var prefix = FindSpecialVersionPrefix(semanticVersionType);
			var expectedSpecialVersion = string.Format("{0}{1}", prefix, string.Join(".", specialVersionParts));

			var sut = new SemanticVersion(major, minor, patch, specialVersionParts, semanticVersionType);

			Assert.Equal(major, sut.Major);
			Assert.Equal(minor, sut.Minor);
			Assert.Equal(patch, sut.Patch);
			Assert.Equal(expectedSpecialVersion, sut.SpecialVersion);
		}

		[Theory]
		[InlineAutoData(SemanticVersionType.PreRelease)]
		[InlineAutoData(SemanticVersionType.Build)]
		public void SpecialVersionsDoesntAllowNullSpecialVersionParts(SemanticVersionType semVerType)
		{
			Assert.Throws<ArgumentNullException>(() => new SemanticVersion(0, 0, 0, null, semVerType));
		}

		private string FindSpecialVersionPrefix(SemanticVersionType semanticVersionType)
		{
			switch (semanticVersionType)
			{
				case SemanticVersionType.PreRelease:
					return "-";
				case SemanticVersionType.Build:
					return "+";
				default:
					return null;
			}
		}
	}
}