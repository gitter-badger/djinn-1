using System;
using System.Collections.Generic;
using Sungiant.Cloud;
using System.Linq;

namespace Sungiant.Djinn
{
	public class MachineBlueprint
	{
		MachineBlueprintSpecification spec;
		
		public MachineBlueprint(MachineBlueprintSpecification spec)
		{
			this.spec = spec;
			this.Security = new CloudSecurityGroup()
			{
				Name = spec.MachineBlueprintId,
				Description = spec.Description,
				Rules = spec.OpenPorts
					.Select(x => new CloudSecurityGroupRule() { Mode = CloudSecurityGroupRule.TransportMode.TCP, Port = x })
					.ToList()
			};
		}
		
		public String Id { get { return spec.MachineBlueprintId; } }
		
		public String Description { get { return spec.Description; } }
		
		public CloudSecurityGroup Security { get; private set; }
		
		// Defines what needs to be done to configure the server
		public List<ActionGroup> Configure { get { return spec.Configure; } }
		
		// Defines what needs to be done to deploy code / dlls / apps to the server
		public List<ActionGroup> Deploy { get { return spec.Deploy; } }
	}
}

