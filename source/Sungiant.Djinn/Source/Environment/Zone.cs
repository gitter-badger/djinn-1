using System;

namespace Sungiant.Djinn
{
	public class Zone
	{
		ZoneSpecification spec;
		
		public Zone(ZoneSpecification spec)
		{
			this.spec = spec;
		}
		
		public String Id { get { return spec.ZoneId; } }
		
		public String Description { get { return spec.Description; } }
		
	}
}

