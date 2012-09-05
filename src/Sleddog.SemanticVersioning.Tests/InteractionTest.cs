using System.Collections.Generic;
using System.Linq;
using Sleddog.SemanticVersioning;
using Xunit;

namespace SemanticVersioning.Tests
{
	public class InteractionTest
	{
		[Fact]
		public void SortWithComparer()
		{
			var expected = new List<SemanticVersion>
			               	{
			               		new SemanticVersion(1, 2, 2),
			               		new SemanticVersion(1, 2, 3, new[] {"aa"}, SemanticVersionType.PreRelease),
			               		new SemanticVersion(1, 2, 3, new[] {"bb"}, SemanticVersionType.PreRelease),
			               		new SemanticVersion(1, 2, 3),
			               		new SemanticVersion(1, 2, 3, new[] {"aa"}, SemanticVersionType.Build),
			               		new SemanticVersion(1, 2, 3, new[] {"bb"}, SemanticVersionType.Build),
			               		new SemanticVersion(1, 2, 4),
			               		new SemanticVersion(1, 3, 3, new[] {"aa"}, SemanticVersionType.PreRelease),
			               		new SemanticVersion(1, 3, 3),
			               		new SemanticVersion(1, 3, 3, new[] {"aa"}, SemanticVersionType.Build)
			               	};

			var actual = new List<SemanticVersion>(expected);

			actual.Shuffle();

			actual.Sort();

			Assert.True(expected.SequenceEqual(actual));
		}
	}
}