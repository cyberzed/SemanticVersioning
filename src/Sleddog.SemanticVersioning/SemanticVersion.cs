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

			VersionType = VersionType.Normal;
		}

		public SemanticVersion(ushort major, ushort minor, ushort patch, IEnumerable<string> specialVersionParts, VersionType versionType)
		{
			if (specialVersionParts == null)
			{
				throw new ArgumentNullException("specialVersionParts");
			}

			var specialPartsList = specialVersionParts.ToList();

			if (specialPartsList.Any() && versionType == VersionType.Normal)
			{
				throw new ArgumentOutOfRangeException("specialVersionParts",
				                                      "SemanticVersioning doesn't allow special versions unless versiontype is PreRelease or Build");
			}

			Major = major;
			Minor = minor;
			Patch = patch;

			ValidateSpecialVersionParts(specialPartsList);

			this.specialVersionParts = specialPartsList;

			VersionType = versionType;
		}

		public ushort Major { get; private set; }
		public ushort Minor { get; private set; }
		public ushort Patch { get; private set; }

		public string SpecialVersion
		{
			get { return FormatSpecialVersion(); }
		}

		public VersionType VersionType { get; private set; }

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}

			var otherSemVer = obj as SemanticVersion;

			if (otherSemVer == null)
			{
				throw new ArgumentException("Can not compare to something that is not a SemanticVersion", "obj");
			}

			return CompareTo(otherSemVer);
		}

		public int CompareTo(SemanticVersion other)
		{
			throw new NotImplementedException();
		}

		public bool Equals(SemanticVersion other)
		{
			throw new NotImplementedException();
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
			switch (VersionType)
			{
				case VersionType.Normal:
					return string.Format("{0}.{1}.{2}", Major, Minor, Patch);
				case VersionType.PreRelease:
				case VersionType.Build:
					return string.Format("{0}.{1}.{2}{3}", Major, Minor, Patch, FormatSpecialVersion());
				default:
					return base.ToString();
			}
		}

		private string FormatSpecialVersion()
		{
			switch (VersionType)
			{
				case VersionType.PreRelease:
				case VersionType.Build:
					return string.Format("{0}{1}", FindVersionTypePrefix(), string.Join(".", specialVersionParts));
				default:
					return null;
			}
		}

		private string FindVersionTypePrefix()
		{
			switch (VersionType)
			{
				case VersionType.PreRelease:
					return "-";
				case VersionType.Build:
					return "+";
				default:
					return string.Empty;
			}
		}
	}
}