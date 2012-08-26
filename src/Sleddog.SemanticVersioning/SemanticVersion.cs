using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sleddog.SemanticVersioning
{
	public class SemanticVersion : IComparable, IComparable<SemanticVersion>, IEquatable<SemanticVersion>
	{
		private static readonly Regex SpecialVersionPartRegex = new Regex(@"[0-9A-Za-z-]+");

		private readonly List<string> specialVersionParts;

		public SemanticVersion(ushort major, ushort minor, ushort patch)
		{
			Major = major;
			Minor = minor;
			Patch = patch;

			SemanticVersionType = SemanticVersionType.Normal;
		}

		public SemanticVersion(ushort major, ushort minor, ushort patch, IEnumerable<string> specialVersionParts,
		                       SemanticVersionType semanticVersionType)
		{
			if (specialVersionParts == null)
			{
				throw new ArgumentNullException("specialVersionParts");
			}

			var specialPartsList = specialVersionParts.ToList();

			if (specialPartsList.Any() && semanticVersionType == SemanticVersionType.Normal)
			{
				throw new ArgumentOutOfRangeException("specialVersionParts",
				                                      "SemanticVersioning doesn't allow special versions unless versiontype is PreRelease or Build");
			}

			Major = major;
			Minor = minor;
			Patch = patch;

			ValidateSpecialVersionParts(specialPartsList);

			this.specialVersionParts = specialPartsList;

			SemanticVersionType = semanticVersionType;
		}

		public ushort Major { get; private set; }
		public ushort Minor { get; private set; }
		public ushort Patch { get; private set; }

		public string SpecialVersion
		{
			get
			{
				if (SemanticVersionType == SemanticVersionType.Normal)
				{
					return null;
				}

				return string.Join(".", specialVersionParts);
			}
		}

		public SemanticVersionType SemanticVersionType { get; private set; }

		public int CompareTo(object obj)
		{
			var otherSemVer = obj as SemanticVersion;

			if (otherSemVer == null)
			{
				throw new ArgumentException("Can not compare to something that is not a SemanticVersion", "obj");
			}

			return CompareTo(otherSemVer);
		}

		public int CompareTo(SemanticVersion other)
		{
			if (this == other)
			{
				return 0;
			}

			if (other == null)
			{
				return 1;
			}

			var basicCompareResult = BasicVersionCompare(other);

			if (basicCompareResult == 0)
			{
				var xVersionType = SemanticVersionType;
				var yVersionType = other.SemanticVersionType;

				if (xVersionType == yVersionType)
				{
					if (xVersionType == SemanticVersionType.Normal)
					{
						return basicCompareResult;
					}

					if (basicCompareResult == 0)
						return string.Compare(SpecialVersion, other.SpecialVersion, StringComparison.InvariantCulture);
				}
				else
				{
					var xVersionValue = (int) xVersionType;
					var yVersionValue = (int) yVersionType;

					return xVersionValue.CompareTo(yVersionValue);
				}
			}

			return basicCompareResult;
		}

		public bool Equals(SemanticVersion other)
		{
			return CompareTo(other) == 0;
		}

		private int BasicVersionCompare(SemanticVersion other)
		{
			if (Major != other.Major)
			{
				return Major.CompareTo(other.Major);
			}

			if (Minor != other.Minor)
			{
				return Minor.CompareTo(other.Minor);
			}

			if (Patch != other.Patch)
			{
				return Patch.CompareTo(other.Patch);
			}

			return 0;
		}

		private void ValidateSpecialVersionParts(IEnumerable<string> versionParts)
		{
			foreach (var versionPart in versionParts)
			{
				if (!SpecialVersionPartRegex.IsMatch(versionPart))
				{
					throw new ArgumentException("SpecialVersionParts must comply with [0-9A-Za-z-]", "versionParts");
				}
			}
		}

		public override string ToString()
		{
			switch (SemanticVersionType)
			{
				case SemanticVersionType.Normal:
					return string.Format("{0}.{1}.{2}", Major, Minor, Patch);
				case SemanticVersionType.PreRelease:
				case SemanticVersionType.Build:
					return string.Format("{0}.{1}.{2}{3}", Major, Minor, Patch, FormatSpecialVersion());
				default:
					return base.ToString();
			}
		}

		private string FormatSpecialVersion()
		{
			switch (SemanticVersionType)
			{
				case SemanticVersionType.PreRelease:
				case SemanticVersionType.Build:
					return string.Format("{0}{1}", FindVersionTypePrefix(), string.Join(".", specialVersionParts));
				default:
					return null;
			}
		}

		private string FindVersionTypePrefix()
		{
			switch (SemanticVersionType)
			{
				case SemanticVersionType.PreRelease:
					return "-";
				case SemanticVersionType.Build:
					return "+";
				default:
					return string.Empty;
			}
		}
	}
}