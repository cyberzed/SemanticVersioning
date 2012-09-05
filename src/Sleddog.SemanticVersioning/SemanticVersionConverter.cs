using System;
using System.Text.RegularExpressions;

namespace Sleddog.SemanticVersioning
{
	public class SemanticVersionConverter
	{
		private static readonly Regex SemVerFormat = new Regex(
			@"^(?<major>\d+)" +
			@"\.(?<minor>\d+)" +
			@"\.(?<patch>\d+)" +
			@"((?<delimiter>[+-])" +
			@"(?<specialVersionParts>[0-9A-Za-z]+(?:\.[0-9A-Za-z]+)*))?$"
			);

		public SemanticVersion Convert(Version ver)
		{
			if (ver == null)
			{
				throw new ArgumentNullException("ver");
			}

			var major = System.Convert.ToUInt16(ver.Major);
			var minor = System.Convert.ToUInt16(ver.Minor);
			var patch = System.Convert.ToUInt16(ver.Build);

			return new SemanticVersion(major, minor, patch);
		}

		public SemanticVersion Convert(string versionString)
		{
			if (versionString == null)
			{
				throw new ArgumentNullException("versionString");
			}

			var semVerMatch = SemVerFormat.Match(versionString);

			if (!semVerMatch.Success)
			{
				throw new ArgumentException("Unable to convert string to SemanticVersion", "versionString");
			}

			var major = System.Convert.ToUInt16(semVerMatch.Groups["major"].Value);
			var minor = System.Convert.ToUInt16(semVerMatch.Groups["minor"].Value);
			var patch = System.Convert.ToUInt16(semVerMatch.Groups["patch"].Value);
			var delimiter = semVerMatch.Groups["delimiter"].Value;
			var specialVersionPartsString = semVerMatch.Groups["specialVersionParts"].Value;

			var semVerType = ConvertDelimiter(delimiter);

			if (semVerType == SemanticVersionType.Normal)
			{
				return new SemanticVersion(major, minor, patch);
			}

			var specialVersionParts = specialVersionPartsString.Split(new[] {"."}, StringSplitOptions.None);

			return new SemanticVersion(major, minor, patch, specialVersionParts, semVerType);
		}

		private SemanticVersionType ConvertDelimiter(string delimiter)
		{
			switch (delimiter)
			{
				case "-":
					return SemanticVersionType.PreRelease;
				case "+":
					return SemanticVersionType.Build;
				default:
					return SemanticVersionType.Normal;
			}
		}
	}
}