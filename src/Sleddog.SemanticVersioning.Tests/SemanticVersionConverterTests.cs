using System;
using Ploeh.AutoFixture.Xunit;
using Sleddog.SemanticVersioning;
using Xunit;
using Xunit.Extensions;

namespace SemanticVersioning.Tests
{
	public class SemanticVersionConverterTests
	{
		[Theory, AutoData]
		public void ConvertsVersion(int major, int minor, int build)
		{
			var ver = new Version(major, minor, build);

			var sut = new SemanticVersionConverter();

			var version = sut.Convert(ver);

			Assert.NotNull(version);
		}

		//add verification of version

		[Theory]
		[InlineAutoData("0.0.0")]
		[InlineAutoData("1.2.3")]
		public void AcceptsNormalVersionString(string semVerString)
		{
			var sut = new SemanticVersionConverter();

			var semVer = sut.Convert(semVerString);

			Assert.NotNull(semVer);
		}

		[Theory]
		[InlineAutoData("0.0.0")]
		[InlineAutoData("1.2.3")]
		public void ConvertsNormalVersionsCorrectly(string semVerString)
		{
			var sut = new SemanticVersionConverter();

			var actual = sut.Convert(semVerString);
		}

		[Theory]
		[InlineAutoData("1.2.3-4")]
		[InlineAutoData("1.2.3-prerelease.4")]
		[InlineAutoData("1.2.3+5")]
		[InlineAutoData("1.2.3+build.5")]
		public void AcceptsSpecialVersionString(string semVerString)
		{
			var sut = new SemanticVersionConverter();

			var semVer = sut.Convert(semVerString);

			Assert.NotNull(semVer);
		}

		private int[] ExtractNormalVersion(string normalVersionString)
		{
			return null;
		}
	}
}