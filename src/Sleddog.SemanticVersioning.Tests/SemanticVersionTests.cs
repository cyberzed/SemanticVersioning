﻿using System;
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
		[InlineAutoData(VersionType.Build)]
		[InlineAutoData(VersionType.PreRelease)]
		public void CanInstantiateWithSpecialVersionWithNonNormalType(VersionType versionType, ushort major, ushort minor, ushort patch,
		                                                              string[] specialVersionParts)
		{
			Assert.DoesNotThrow(() => new SemanticVersion(major, minor, patch, specialVersionParts, versionType));
		}

		[Theory]
		[InlineAutoData(VersionType.Normal)]
		public void CanNotInstantiateWithSpecialVersionWithNormalType(VersionType versionType, ushort major, ushort minor, ushort patch,
		                                                              string[] specialVersionParts)
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new SemanticVersion(major, minor, patch, specialVersionParts, versionType));
		}

		[Theory]
		[InlineAutoData(VersionType.Build)]
		public void ThrowsExceptionOnMalformedSpecialVersionParts(VersionType versionType, ushort major, ushort minor, ushort patch,
		                                                          string[] specialVersionParts)
		{
			var malformedSpecialPart = ".,";

			var versionParts = specialVersionParts.Concat(new[] {malformedSpecialPart});

			Assert.Throws<ArgumentException>(() => new SemanticVersion(major, minor, patch, versionParts, versionType));
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
		[InlineAutoData(VersionType.Build)]
		[InlineAutoData(VersionType.PreRelease)]
		public void SpecialVersionsGetsSetCorrectly(VersionType versionType, ushort major, ushort minor, ushort patch,
		                                            string[] specialVersionParts)
		{
			var prefix = FindSpecialVersionPrefix(versionType);
			var expectedSpecialVersion = string.Format("{0}{1}", prefix, string.Join(".", specialVersionParts));

			var sut = new SemanticVersion(major, minor, patch, specialVersionParts, versionType);

			Assert.Equal(major, sut.Major);
			Assert.Equal(minor, sut.Minor);
			Assert.Equal(patch, sut.Patch);
			Assert.Equal(expectedSpecialVersion, sut.SpecialVersion);
		}

		private string FindSpecialVersionPrefix(VersionType versionType)
		{
			switch (versionType)
			{
				case VersionType.PreRelease:
					return "-";
				case VersionType.Build:
					return "+";
				default:
					return null;
			}
		}
	}
}