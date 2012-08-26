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
			var expected = new List<SemanticVersion>();

			var normal4 = new SemanticVersion(1, 2, 2);
			var preRelease1 = new SemanticVersion(1, 2, 3, new[] {"aa"}, SemanticVersionType.PreRelease);
			var preRelease3 = new SemanticVersion(1, 2, 3, new[] {"bb"}, SemanticVersionType.PreRelease);
			var normal1 = new SemanticVersion(1, 2, 3);
			var build1 = new SemanticVersion(1, 2, 3, new[] {"aa"}, SemanticVersionType.Build);
			var build3 = new SemanticVersion(1, 2, 3, new[] {"bb"}, SemanticVersionType.Build);
			var normal3 = new SemanticVersion(1, 2, 4);
			var preRelease2 = new SemanticVersion(1, 3, 3, new[] {"aa"}, SemanticVersionType.PreRelease);
			var normal2 = new SemanticVersion(1, 3, 3);
			var build2 = new SemanticVersion(1, 3, 3, new[] {"aa"}, SemanticVersionType.Build);

			expected.Add(normal4);
			expected.Add(preRelease1);
			expected.Add(preRelease3);
			expected.Add(normal1);
			expected.Add(build1);
			expected.Add(build3);
			expected.Add(normal3);
			expected.Add(preRelease2);
			expected.Add(normal2);
			expected.Add(build2);

			var actual = new List<SemanticVersion>(expected);

			actual.Shuffle();

			actual.Sort();

			Assert.True(expected.SequenceEqual(actual));
		}
	}
}