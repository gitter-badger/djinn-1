using System;

namespace Sungiant.Djinn.Specification
{
	public class NginxServerBlock
		: IAction
	{
		// Base class data
		public String Type { get; set; }
		public String Description { get; set; }

		// NginxServerBlockSpecification data
		public String Name { get; set; }
		public String Listen { get; set; }
		public String[] VirtualHosts { get; set; }
		public String[] AllowedEndpoints { get; set; }
		public INginxLocationBlock[] Locations { get; set; }
		public SslConfig SslConfig { get; set; }
		public String Return { get; set; }
	}

	public interface INginxLocationBlock
	{
		String Type { get; set; }
		String Location { get; set; }
	}

	public class ReturnLocationBlock
		: INginxLocationBlock
	{
		// Base class data
		public String Type { get; set; }
		public String Location { get; set; }

		// NginxReturnLocationBlock data
		public String Return { get; set; }
	}

	public class StaticLocationBlock
		: INginxLocationBlock
	{
		// Base class data
		public String Type { get; set; }
		public String Location { get; set; }

		// NginxStaticLocationBlock data
		public String Root { get; set; }
		public String Index { get; set; }
		public String TryFiles { get; set; }
		public String Rewrite { get; set; }
	}

	public class ProxyPassLocationBlock
		: INginxLocationBlock
	{
		// Base class data
		public String Type { get; set; }
		public String Location { get; set; }

		// NginxProxyPassLocationBlock data
		public String ProxyPass { get; set; }
		public String Rewrite { get; set; }
	}

	public class SslConfig
	{
		public String Certificate { get; set; }
		public String CertificateKey { get; set; }
	}
}

