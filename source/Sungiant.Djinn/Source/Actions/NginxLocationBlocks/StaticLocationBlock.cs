using System;

namespace Sungiant.Djinn.Actions
{
	public class StaticLocationBlock
		: NginxLocationBlock<Specification.StaticLocationBlock>
	{
		public StaticLocationBlock(Specification.StaticLocationBlock specification)
			: base(specification) {}

		public override String[] GetConfig()
		{
			return new String[]
			{
				"location " + Specification.Location + " {",
				Specification.Root == null ? String.Empty : "  root  " + Specification.Root + ";",
				Specification.Rewrite == null ? String.Empty : "  rewrite " + Specification.Rewrite + ";",
				Specification.Index == null ? String.Empty : "  index " + Specification.Index + ";",
				Specification.TryFiles == null ? String.Empty : "  try_files " + Specification.TryFiles + ";",
				"}",
			};
		}
	}
}

