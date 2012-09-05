using Sleddog.SemanticVersioning;

namespace SemanticVersioning.Tests
{
	internal class VersionResult
	{
		public VersionResult()
		{
			SpecialVersion = string.Empty;
		}

		public ushort Major { get; set; }
		public ushort Minor { get; set; }
		public ushort Patch { get; set; }
		public SemanticVersionType SemanticVersionType { get; set; }
		public string SpecialVersion { get; set; }
	}
}