using System;

namespace Sungiant.Djinn
{
	public class Zone
	{
		readonly Specification.Zone spec;
		
		public Zone(Specification.Zone spec)
		{
			this.spec = spec;
		}
		
		public String Identifier { get { return spec.ZoneIdentifier; } }
		
		public String Description { get { return spec.Description; } }
		
	}
}

