using System;
using Ploeh.AutoFixture.Xunit;
using Sleddog.SemanticVersioning;
using Xunit;
using Xunit.Extensions;

namespace SemanticVersioning.Tests
{
	public class SemanticVersionBuilderTests
	{
		[Theory, AutoData]
		public void AcceptsVersion(int major, int minor, int build)
		{
			var ver = new Version(major,minor,build);

			var sut = new SemanticVersionBuilder();

			var semVer = sut.Convert(ver);

			Assert.NotNull(semVer);
		}
	}
}