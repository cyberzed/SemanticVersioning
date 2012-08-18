using System;
using System.Collections.Generic;

namespace Sleddog.SemanticVersioning
{
	public class SemanticVersionComparer : IComparer<SemanticVersion>
	{
		private readonly IDictionary<SemanticVersionType, Func<SemanticVersion, SemanticVersion, int>> comparisons =
			new Dictionary<SemanticVersionType, Func<SemanticVersion, SemanticVersion, int>>();

		public SemanticVersionComparer()
		{
			comparisons.Add(SemanticVersionType.PreRelease, ComparePreReleaseVersion);
			comparisons.Add(SemanticVersionType.Normal, CompareNormalVersion);
			comparisons.Add(SemanticVersionType.Build, CompareBuildVersion);
		}

		public int Compare(SemanticVersion x, SemanticVersion y)
		{
			if (x == y)
			{
				return 0;
			}

			if (x == null)
			{
				return -1;
			}

			if (y == null)
			{
				return 1;
			}

			var xVersionType = x.SemanticVersionType;
			var yVersionType = y.SemanticVersionType;

			if (xVersionType == yVersionType)
			{
				return comparisons[xVersionType](x, y);
			}
			else
			{
				var xVersionValue = (int) xVersionType;
				var yVersionValue = (int) yVersionType;

				return xVersionValue.CompareTo(yVersionValue);
			}
		}

		private int ComparePreReleaseVersion(SemanticVersion x, SemanticVersion y)
		{
			return -1;
		}

		private int CompareNormalVersion(SemanticVersion x, SemanticVersion y)
		{
			if (x.Major != y.Major)
			{
				return x.Major.CompareTo(y.Major);
			}

			if (x.Minor != y.Minor)
			{
				return x.Minor.CompareTo(y.Minor);
			}

			if (x.Patch != y.Patch)
			{
				return x.Patch.CompareTo(y.Patch);
			}

			return 0;
		}

		private int CompareBuildVersion(SemanticVersion x, SemanticVersion y)
		{
			return -1;
		}
	}
}