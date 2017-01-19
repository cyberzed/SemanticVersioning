using System;
using Ploeh.AutoFixture.Xunit2;
using Sleddog.SemanticVersioning;
using Xunit;

namespace SemanticVersioning.Tests
{
    public class SemanticVersionComparerTests
    {
        [Theory]
        [AutoData]
        public void SameSemanticVersionIsEqual(SemanticVersion semVer)
        {
            const int expected = 0;

            var actual = semVer.CompareTo(semVer);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [AutoData]
        public void SameSemanticVersionNumberIsEqual(ushort major, ushort minor, ushort patch)
        {
            var semVer1 = new SemanticVersion(major, minor, patch);
            var semVer2 = new SemanticVersion(major, minor, patch);

            var expected = 0;
            var actual = semVer1.CompareTo(semVer2);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [AutoData]
        public void SmallerVersionYieldsNegativeValue(SemanticVersion semVer1, SemanticVersion semVer2)
        {
            var actual = semVer1.CompareTo(semVer2);

            Assert.True(actual < 0);
        }

        [Theory]
        [AutoData]
        public void HigherVersionYieldsPositiveValue(SemanticVersion semVer1, SemanticVersion semVer2)
        {
            var actual = semVer2.CompareTo(semVer1);

            Assert.True(actual > 0);
        }

        [Theory]
        [InlineAutoData(SemanticVersionType.PreRelease, SemanticVersionType.Normal, -1)]
        [InlineAutoData(SemanticVersionType.PreRelease, SemanticVersionType.Build, -1)]
        [InlineAutoData(SemanticVersionType.Normal, SemanticVersionType.Build, -1)]
        public void DifferentVersionTypesArentEqual(SemanticVersionType semVerType1, SemanticVersionType semVerType2,
                                                    int expected)
        {
            var semVer1 = CreateZeroVersion(semVerType1);
            var semVer2 = CreateZeroVersion(semVerType2);

            var actual = semVer1.CompareTo(semVer2);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [AutoData]
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
            var semVer1 = new SemanticVersion(1, 2, 3, new[] {"aa"}, semVerType);
            var semVer2 = new SemanticVersion(1, 2, 3, new[] {"bb"}, semVerType);

            const int expected = -1;

            var actual = semVer1.CompareTo(semVer2);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineAutoData(SemanticVersionType.PreRelease)]
        [InlineAutoData(SemanticVersionType.Build)]
        public void BiggerAlphaNumericPreReleaseYieldsPositiveResult(SemanticVersionType semVerType)
        {
            var semVer1 = new SemanticVersion(1, 2, 3, new[] {"bb"}, semVerType);
            var semVer2 = new SemanticVersion(1, 2, 3, new[] {"aa"}, semVerType);

            const int expected = 1;

            var actual = semVer1.CompareTo(semVer2);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [AutoData]
        public void SemVerDoesntCompareWithValueTypes(SemanticVersion semVer)
        {
            Assert.Throws<ArgumentException>(() => semVer.CompareTo(5));
        }

        [Theory]
        [AutoData]
        public void SemVerDoesntCompareWithReferenceTypes(SemanticVersion semVer)
        {
            Assert.Throws<ArgumentException>(() => semVer.CompareTo(new EventArgs()));
        }

        [Theory]
        [AutoData]
        public void SemVerComparesWellWithUnboxing(SemanticVersion semVer)
        {
            object input = semVer;

            const int expected = 0;

            var actual = semVer.CompareTo(input);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [AutoData]
        public void DifferentSemVersArentEqual(SemanticVersion semVer1, SemanticVersion semVer2)
        {
            const bool expected = true;

            var actual = semVer1 != semVer2;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [AutoData]
        public void SemVerArentEqualToNull(SemanticVersion semVer)
        {
            const bool expected = false;

            var actual = semVer.Equals(null);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [AutoData]
        public void SemVerArentEqualToStringEmpty(SemanticVersion semVer)
        {
            const bool expected = false;

            var actual = semVer.Equals(string.Empty);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [AutoData]
        public void SemVerEqualsItself(SemanticVersion semVer)
        {
            const bool expected = true;

            var actual = semVer.Equals(semVer);

            Assert.Equal(expected, actual);
        }

        private SemanticVersion CreateZeroVersion(SemanticVersionType semVerType)
        {
            if (semVerType == SemanticVersionType.Normal)
                return new SemanticVersion(0, 0, 0);

            return new SemanticVersion(0, 0, 0, new[] {"0"}, semVerType);
        }
    }
}