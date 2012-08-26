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
			var expectedSpecialVersion = string.Join(".", specialVersionParts);

			var sut = new SemanticVersion(major, minor, patch, specialVersionParts, semanticVersionType);

			Assert.Equal(major, sut.Major);
			Assert.Equal(minor, sut.Minor);
			Assert.Equal(patch, sut.Patch);
			Assert.Equal(expectedSpecialVersion, sut.SpecialVersion);
		}

		[Theory]
		[InlineAutoData(SemanticVersionType.PreRelease)]
		[InlineAutoData(SemanticVersionType.Build)]
		public void SpecialVersionsDoesntAllowNullSpecialVersionParts(SemanticVersionType semVerType, ushort major, ushort minor, ushort patch)
		{
			Assert.Throws<ArgumentNullException>(() => new SemanticVersion(major, minor, patch, null, semVerType));
		}

		[Theory]
		[InlineAutoData(SemanticVersionType.PreRelease)]
		[InlineAutoData(SemanticVersionType.Build)]
		public void ToStringFormatsCorrectlyWithSpecialVersions(SemanticVersionType semVerType, ushort major, ushort minor, ushort patch,
		                                                        string[] specialVersions)
		{
			var semVer = new SemanticVersion(major, minor, patch, specialVersions, semVerType);

			var expected = FormatVersion(semVer);

			var actual = semVer.ToString();

			Assert.Equal(expected, actual);
		}

		[Theory, AutoData]
		public void ToStringFormatsCorrectlyWithNormalVersions(SemanticVersion semVer)
		{
			var expected = FormatVersion(semVer);

			var actual = semVer.ToString();

			Assert.Equal(expected, actual);
		}

		[Theory, AutoData]
		public void SameSemanticVersionIsEqual(SemanticVersion semVer)
		{
			const int expected = 0;

			var actual = semVer.CompareTo(semVer);

			Assert.Equal(expected, actual);
		}

		[Theory, AutoData]
		public void SameSemanticVersionNumberIsEqual(ushort major, ushort minor, ushort patch)
		{
			var semVer1 = new SemanticVersion(major, minor, patch);
			var semVer2 = new SemanticVersion(major, minor, patch);

			var expected = 0;
			var actual = semVer1.CompareTo(semVer2);

			Assert.Equal(expected, actual);
		}

		[Theory, AutoData]
		public void SmallerVersionYieldsNegativeValue(SemanticVersion semVer1, SemanticVersion semVer2)
		{
			var actual = semVer1.CompareTo(semVer2);

			Assert.True(actual < 0);
		}

		[Theory, AutoData]
		public void HigherVersionYieldsPositiveValue(SemanticVersion semVer1, SemanticVersion semVer2)
		{
			var actual = semVer2.CompareTo(semVer1);

			Assert.True(actual > 0);
		}

		[Theory]
		[InlineAutoData(SemanticVersionType.PreRelease, SemanticVersionType.Normal, -1)]
		[InlineAutoData(SemanticVersionType.PreRelease, SemanticVersionType.Build, -1)]
		[InlineAutoData(SemanticVersionType.Normal, SemanticVersionType.Build, -1)]
		public void DifferentVersionTypesArentEqual(SemanticVersionType semVerType1, SemanticVersionType semVerType2, int expected)
		{
			var semVer1 = CreateZeroVersion(semVerType1);
			var semVer2 = CreateZeroVersion(semVerType2);

			var actual = semVer1.CompareTo(semVer2);

			Assert.Equal(expected, actual);
		}

		[Theory, AutoData]
		public void XCompareToNullGivesAPositiveValue(SemanticVersion semVer)
		{
			var actual = semVer.CompareTo(null);

			Assert.True(actual > 0);
		}

		[Theory]
		[InlineAutoData(SemanticVersionType.PreRelease)]
		[InlineAutoData(SemanticVersionType.Build)]
		public void SmallerAlphanumericPreReleaseYieldsNegativeResult(SemanticVersionType semVerType)
		{
			var semVer1 = new SemanticVersion(1, 2, 3, new[] { "aa" }, semVerType);
			var semVer2 = new SemanticVersion(1, 2, 3, new[] { "bb" }, semVerType);

			var expected = -1;

			var actual = semVer1.CompareTo(semVer2);

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineAutoData(SemanticVersionType.PreRelease)]
		[InlineAutoData(SemanticVersionType.Build)]
		public void BiggerAlphaNumericPreReleaseYieldsPositiveResult(SemanticVersionType semVerType)
		{
			var semVer1 = new SemanticVersion(1, 2, 3, new[] { "bb" }, semVerType);
			var semVer2 = new SemanticVersion(1, 2, 3, new[] { "aa" }, semVerType);

			var expected = 1;

			var actual = semVer1.CompareTo(semVer2);

			Assert.Equal(expected, actual);
		}

		[Theory, AutoData]
		public void SemVerDoesntCompareWithValueTypes(SemanticVersion semVer)
		{
			Assert.Throws<ArgumentException>(() => semVer.CompareTo(5));
		}

		[Theory, AutoData]
		public void SemVerDoesntCompareWithReferenceTypes(SemanticVersion semVer)
		{
			Assert.Throws<ArgumentException>(() => semVer.CompareTo(new EventArgs()));
		}

		private SemanticVersion CreateZeroVersion(SemanticVersionType semVerType)
		{
			if (semVerType == SemanticVersionType.Normal)
			{
				return new SemanticVersion(0, 0, 0);
			}

			return new SemanticVersion(0, 0, 0, new[] { "0" }, semVerType);
		}

		private string FormatVersion(SemanticVersion semVer)
		{
			switch (semVer.SemanticVersionType)
			{
				case SemanticVersionType.PreRelease:
					return string.Format("{0}.{1}.{2}-{3}", semVer.Major, semVer.Minor, semVer.Patch, semVer.SpecialVersion);
				case SemanticVersionType.Build:
					return string.Format("{0}.{1}.{2}+{3}", semVer.Major, semVer.Minor, semVer.Patch, semVer.SpecialVersion);
				default:
					return string.Format("{0}.{1}.{2}", semVer.Major, semVer.Minor, semVer.Patch);
			}
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