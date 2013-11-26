using System;

namespace Sungiant.Djinn
{
	public class ProxyPassLocationBlock
		: NginxLocationBlock<Specification.ProxyPassLocationBlock>
	{
		public ProxyPassLocationBlock(Specification.ProxyPassLocationBlock specification)
			: base(specification) {}

		public override String[] GetConfig()
		{
			return new String[]
			{
				"location " + Specification.Location + " {",
				Specification.Rewrite == null ? String.Empty : "  rewrite " + Specification.Rewrite + ";",
				"  proxy_pass " + Specification.ProxyPass + ";",
				"  proxy_set_header  X-Remote-Address  $remote_addr;",
				"  proxy_set_header  X-Host  $host;",
				"}",
			};
		}
	}
}

