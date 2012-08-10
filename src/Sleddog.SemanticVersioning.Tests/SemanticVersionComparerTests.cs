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
	}
}