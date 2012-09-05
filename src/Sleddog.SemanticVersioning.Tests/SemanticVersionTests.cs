using System;
using System.Linq;
using Ploeh.AutoFixture.Xunit;
using Sleddog.SemanticVersioning;
using Xunit;
using Xunit.Extensions;
using Ploeh.SemanticComparison.Fluent;

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
			const string malformedSpecialPart = ".,";

			var versionParts = specialVersionParts.Concat(new[] {malformedSpecialPart});

			Assert.Throws<ArgumentException>(() => new SemanticVersion(major, minor, patch, versionParts, semanticVersionType));
		}

		[Theory, AutoData]
		public void NormalTypeValuesGetsSetCorrectly(ushort major, ushort minor, ushort patch)
		{
			var expected = new VersionResult {Major = major, Minor = minor, Patch = patch};

			var actual = new SemanticVersion(major, minor, patch);

			actual.AsSource().OfLikeness<VersionResult>().ShouldEqual(expected);
		}

		[Theory]
		[InlineAutoData(SemanticVersionType.Build)]
		[InlineAutoData(SemanticVersionType.PreRelease)]
		public void SpecialVersionsGetsSetCorrectly(SemanticVersionType semanticVersionType, ushort major, ushort minor, ushort patch,
		                                            string[] specialVersionParts)
		{
			var expectedSpecialVersion = string.Join(".", specialVersionParts);

			var expected = new VersionResult
			               	{
			               		Major = major,
			               		Minor = minor,
			               		Patch = patch,
			               		SemanticVersionType = semanticVersionType,
			               		SpecialVersion = expectedSpecialVersion
			               	};

			var actual = new SemanticVersion(major, minor, patch, specialVersionParts, semanticVersionType);

			actual.AsSource().OfLikeness<VersionResult>().ShouldEqual(expected);
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
		public void NormalTypeHashCodeVerification(ushort major, ushort minor, ushort patch)
		{
			var semVer = new SemanticVersion(major, minor, patch);

			var expected = GetSemVerHashCode(semVer);

			var actual = semVer.GetHashCode();

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineAutoData(SemanticVersionType.PreRelease)]
		[InlineAutoData(SemanticVersionType.Build)]
		public void SpecialTypeHashCodeVerification(SemanticVersionType semVerType, ushort major, ushort minor, ushort patch,
		                                            string[] specialParts)
		{
			var semVer = new SemanticVersion(major, minor, patch, specialParts, semVerType);

			var expected = GetSemVerHashCode(semVer);

			var actual = semVer.GetHashCode();

			Assert.Equal(expected, actual);
		}

		private int GetSemVerHashCode(SemanticVersion semVer)
		{
			var hashCode = semVer.Major.GetHashCode();

			hashCode = (hashCode*397) ^ semVer.Minor.GetHashCode();
			hashCode = (hashCode*397) ^ semVer.Patch.GetHashCode();
			hashCode = (hashCode*397) ^ semVer.SpecialVersion.GetHashCode();
			hashCode = (hashCode*397) ^ semVer.SemanticVersionType.GetHashCode();

			return hashCode;
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
	}
}