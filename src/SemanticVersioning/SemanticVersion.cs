using System;
using System.Linq;

namespace Sleddog.SemanticVersioning
{
	public class SemanticVersion
	{
		private readonly string[] specialVersionParts;

		public SemanticVersion(ushort major, ushort minor, ushort patch)
		{
			Major = major;
			Minor = minor;
			Patch = patch;

			VersionType = VersionType.Normal;
		}

		public SemanticVersion(ushort major, ushort minor, ushort patch, string[] specialVersionParts, VersionType versionType)
		{
			if (specialVersionParts.Any() && versionType == VersionType.Normal)
			{
				throw new ArgumentOutOfRangeException("specialVersionParts",
				                                      "SemanticVersioning doesn't allow special versions unless versiontype is PreRelease or Build");
			}

			Major = major;
			Minor = minor;
			Patch = patch;

			this.specialVersionParts = specialVersionParts;

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
					return string.Empty;
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