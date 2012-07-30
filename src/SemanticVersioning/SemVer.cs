using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace SemanticVersioning
{
	public class SemVer
	{
		private const string BuildPrefix = "build";

		public SemVer(Version version)
		{
			Major = Convert.ToUInt16(version.Major);
			Minor = Convert.ToUInt16(version.Minor);

			var specialVersion = new StringBuilder(BuildPrefix);

			if (version.Build > 0)
			{
				specialVersion.AppendFormat(".{0}", version.Build);
			}

			if (version.Revision > 0)
			{
				specialVersion.AppendFormat(".{0}", version.Revision);
			}

			if (specialVersion.Length > BuildPrefix.Length)
			{
				SpecialVersion = specialVersion.ToString();
				VersionType = VersionType.Build;
			}
		}

		public SemVer(ushort major, ushort minor, ushort patch, string specialVersion)
		{
			Major = major;
			Minor = minor;
			Patch = patch;
			SpecialVersion = specialVersion;
		}

		public ushort Major { get; private set; }
		public ushort Minor { get; private set; }
		public ushort Patch { get; private set; }
		public string SpecialVersion { get; private set; }
		public VersionType VersionType { get; private set; }

		private bool ValidateSpecialVersion(string specialVersion)
		{
			var specialVersionRegex = new Regex(@"^(?<prefix>[\+-])");

			return false;
		}

		public override string ToString()
		{
			switch (VersionType)
			{
				case VersionType.Normal:
					return string.Format("{0}.{1}.{2}", Major, Minor, Patch);
				case VersionType.PreRelease:
					return string.Format("{0}.{1}.{2}-{3}", Major, Minor, Patch, SpecialVersion);
				case VersionType.Build:
					return string.Format("{0}.{1}.{2}+{3}", Major, Minor, Patch, SpecialVersion);
				default:
					return base.ToString();
			}
		}
	}
}