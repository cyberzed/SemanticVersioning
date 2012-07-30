using System;
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
	}
}