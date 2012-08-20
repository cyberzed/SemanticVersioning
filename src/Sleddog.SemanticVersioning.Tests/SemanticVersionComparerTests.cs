using Ploeh.AutoFixture.Xunit;
using Sleddog.SemanticVersioning;
using Xunit;
using Xunit.Extensions;

namespace SemanticVersioning.Tests
{
	public class SemanticVersionComparerTests
	{
		[Theory, AutoData]
		public void CanInstantiate()
		{
			Assert.DoesNotThrow(() => new SemanticVersionComparer());
		}

		[Theory, AutoData]
		public void SameSemanticVersionIsEqual(SemanticVersion semVer)
		{
			var sut = new SemanticVersionComparer();

			const int expected = 0;

			var actual = sut.Compare(semVer, semVer);

			Assert.Equal(expected, actual);
		}

		[Theory, AutoData]
		public void SameSemanticVersionNumberIsEqual(ushort major, ushort minor, ushort patch)
		{
			var semVer1 = new SemanticVersion(major, minor, patch);
			var semVer2 = new SemanticVersion(major, minor, patch);

			var sut = new SemanticVersionComparer();

			var expected = 0;
			var actual = sut.Compare(semVer1, semVer2);

			Assert.Equal(expected, actual);
		}

		[Theory, AutoData]
		public void SmallerVersionYieldsNegativeValue(SemanticVersion semVer1, SemanticVersion semVer2)
		{
			var sut = new SemanticVersionComparer();

			var actual = sut.Compare(semVer1, semVer2);

			Assert.True(actual < 0);
		}

		[Theory, AutoData]
		public void HigherVersionYieldsPositiveValue(SemanticVersion semVer1, SemanticVersion semVer2)
		{
			var sut = new SemanticVersionComparer();

			var actual = sut.Compare(semVer2, semVer1);

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

			var sut = new SemanticVersionComparer();

			var actual = sut.Compare(semVer1, semVer2);

			Assert.Equal(expected, actual);
		}

		[Theory, AutoData]
		public void XCompareToNullGivesAPositiveValue(SemanticVersion semVer)
		{
			var sut = new SemanticVersionComparer();

			var actual = sut.Compare(semVer, null);

			Assert.True(actual > 0);
		}

		[Theory, AutoData]
		public void YComparetToNullGivesANegativeValue(SemanticVersion semVer)
		{
			var sut = new SemanticVersionComparer();

			var actual = sut.Compare(null, semVer);

			Assert.True(actual < 0);
		}

		[Theory]
		[InlineAutoData(SemanticVersionType.PreRelease)]
		[InlineAutoData(SemanticVersionType.Build)]
		public void SmallerAlphanumericPreReleaseYieldsNegativeResult(SemanticVersionType semVerType)
		{
			var semVer1 = new SemanticVersion(1, 2, 3, new[] {"aa"}, semVerType);
			var semVer2 = new SemanticVersion(1, 2, 3, new[] {"bb"}, semVerType);

			var expected = -1;

			var sut = new SemanticVersionComparer();

			var actual = sut.Compare(semVer1, semVer2);

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineAutoData(SemanticVersionType.PreRelease)]
		[InlineAutoData(SemanticVersionType.Build)]
		public void BiggerAlphaNumericPreReleaseYieldsPositiveResult(SemanticVersionType semVerType)
		{
			var semVer1 = new SemanticVersion(1, 2, 3, new[] {"bb"}, semVerType);
			var semVer2 = new SemanticVersion(1, 2, 3, new[] {"aa"}, semVerType);

			var expected = 1;

			var sut = new SemanticVersionComparer();

			var actual = sut.Compare(semVer1, semVer2);

			Assert.Equal(expected, actual);
		}

		private SemanticVersion CreateZeroVersion(SemanticVersionType semVerType)
		{
			if (semVerType == SemanticVersionType.Normal)
			{
				return new SemanticVersion(0, 0, 0);
			}

			return new SemanticVersion(0, 0, 0, new[] {"0"}, semVerType);
		}
	}
}