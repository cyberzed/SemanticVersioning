using System;
using System.Text.RegularExpressions;
using Ploeh.AutoFixture.Xunit;
using Ploeh.SemanticComparison.Fluent;
using Sleddog.SemanticVersioning;
using Xunit;
using Xunit.Extensions;

namespace SemanticVersioning.Tests
{
	public class SemanticVersionConverterTests
	{
		private static readonly Regex NormalVersionRegex = new Regex(@"^(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)$");

		private static readonly Regex SpecialVersionRegex = new Regex(
			@"^(?<major>\d+)" +
			@"\.(?<minor>\d+)" +
			@"\.(?<patch>\d+)" +
			@"(?<delimiter>[-+])(?<specialVersionParts>[0-9A-Za-z]+(?:\.[0-9A-Za-z])*)$"
			);

		[Theory, AutoData]
		public void AcceptsVersion(int major, int minor, int build)
		{
			var version = new Version(major, minor, build);

			var sut = new SemanticVersionConverter();

			var actual = sut.Convert(version);

			Assert.NotNull(actual);
		}

		[Theory]
		[InlineAutoData(0, 0, 0)]
		[InlineAutoData(1, 2, 3)]
		[InlineAutoData(10, 20, 30)]
		public void ConvertsVersionCorrectly(int major, int minor, int build)
		{
			var expected = new VersionResult {Major = (ushort) major, Minor = (ushort) minor, Patch = (ushort) build};

			var version = new Version(major, minor, build);

			var sut = new SemanticVersionConverter();

			var actual = sut.Convert(version);

			actual.AsSource().OfLikeness<VersionResult>().ShouldEqual(expected);
		}

		[Theory]
		[InlineAutoData(int.MaxValue, 0, 0)]
		[InlineAutoData(0, int.MaxValue, 0)]
		[InlineAutoData(0, 0, int.MaxValue)]
		public void DoesntAcceptValuesBiggerThanUShort(int major, int minor, int build)
		{
			var version = new Version(major, minor, build);

			var sut = new SemanticVersionConverter();

			Assert.Throws<ArgumentOutOfRangeException>(() => sut.Convert(version));
		}

		[Theory]
		[InlineAutoData("0.0.0")]
		[InlineAutoData("1.2.3")]
		public void AcceptsNormalVersionString(string normalVersionString)
		{
			var sut = new SemanticVersionConverter();

			var actual = sut.Convert(normalVersionString);

			Assert.NotNull(actual);
		}

		[Theory]
		[InlineAutoData("0.0.0")]
		[InlineAutoData("1.2.3")]
		public void ConvertsNormalVersionsCorrectly(string normalVersionString)
		{
			var sut = new SemanticVersionConverter();

			var expected = ExtractNormalVersion(normalVersionString);

			var actual = sut.Convert(normalVersionString);

			actual.AsSource().OfLikeness<VersionResult>().ShouldEqual(expected);
		}

		[Theory]
		[InlineAutoData("1.2.3-4")]
		[InlineAutoData("1.2.3-prerelease.4")]
		[InlineAutoData("1.2.3+5")]
		[InlineAutoData("1.2.3+build.5")]
		public void AcceptsSpecialVersionString(string specialVersionString)
		{
			var sut = new SemanticVersionConverter();

			var actual = sut.Convert(specialVersionString);

			Assert.NotNull(actual);
		}

		[Theory]
		[InlineAutoData("1.2.3-4")]
		[InlineAutoData("1.2.3-prerelease.4")]
		[InlineAutoData("1.2.3+5")]
		[InlineAutoData("1.2.3+build.5")]
		public void ConvertsSpecialVersionCorrectly(string specialVersionString)
		{
			var sut = new SemanticVersionConverter();

			var expected = ExtractSpecialVersion(specialVersionString);

			var actual = sut.Convert(specialVersionString);

			actual.AsSource().OfLikeness<VersionResult>().ShouldEqual(expected);
		}

		private VersionResult ExtractNormalVersion(string normalVersionString)
		{
			var versionMatch = NormalVersionRegex.Match(normalVersionString);

			if (versionMatch.Success)
			{
				var major = Convert.ToUInt16(versionMatch.Groups["major"].Value);
				var minor = Convert.ToUInt16(versionMatch.Groups["minor"].Value);
				var patch = Convert.ToUInt16(versionMatch.Groups["patch"].Value);

				var versionResult = new VersionResult
				                    	{
				                    		Major = major,
				                    		Minor = minor,
				                    		Patch = patch
				                    	};

				return versionResult;
			}

			return null;
		}

		private VersionResult ExtractSpecialVersion(string specialVersionString)
		{
			var versionMatch = SpecialVersionRegex.Match(specialVersionString);

			if (versionMatch.Success)
			{
				var major = Convert.ToUInt16(versionMatch.Groups["major"].Value);
				var minor = Convert.ToUInt16(versionMatch.Groups["minor"].Value);
				var patch = Convert.ToUInt16(versionMatch.Groups["patch"].Value);
				var versionType = ParseDelimiter(versionMatch.Groups["delimiter"].Value);
				var specialVersion = versionMatch.Groups["specialVersionParts"].Value;

				var versionResult = new VersionResult
				                    	{
				                    		Major = major,
				                    		Minor = minor,
				                    		Patch = patch,
				                    		SemanticVersionType = versionType,
				                    		SpecialVersion = specialVersion
				                    	};

				return versionResult;
			}

			return null;
		}

		private SemanticVersionType ParseDelimiter(string delimiter)
		{
			switch (delimiter)
			{
				case "-":
					return SemanticVersionType.PreRelease;
				case "+":
					return SemanticVersionType.Build;
				default:
					return SemanticVersionType.Normal;
			}
		}
	}
}