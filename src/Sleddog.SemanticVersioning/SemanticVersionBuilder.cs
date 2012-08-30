using System;

namespace Sleddog.SemanticVersioning
{
	public class SemanticVersionBuilder
	{
		public SemanticVersion Convert(Version ver)
		{
			var major = System.Convert.ToUInt16(ver.Major);
			var minor = System.Convert.ToUInt16(ver.Minor);
			var patch = System.Convert.ToUInt16(ver.Build);

			return new SemanticVersion(major, minor, patch);
		}
	}
}