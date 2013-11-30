using System;
using System.Collections.Generic;
using Sungiant.Cloud;
using System.Linq;

namespace Sungiant.Djinn
{
	public class Blueprint
	{
		/// <summary>
		/// The spec file.
		/// </summary>
		readonly Specification.Blueprint specification;
		readonly CloudSecurityGroup securityGroup;

		readonly List<ActionGroup> configurationActions;
		readonly List<ActionGroup> deploymentActions;

		public Blueprint(Specification.Blueprint specification, String djinnContext)
		{
			this.specification = specification;
			this.securityGroup = new CloudSecurityGroup()
			{
				Name = this.specification.BlueprintIdentifier,
				Description = this.specification.Description ?? this.specification.BlueprintIdentifier,
				Rules = this.specification.OpenPorts
					.Select(x => new CloudSecurityGroupRule() { Mode = CloudSecurityGroupRule.TransportMode.TCP, Port = x })
					.ToList()
			};

			if (specification.Configuration.ActionGroups != null)
				this.configurationActions = this.specification.Configuration.ActionGroups.Select( x => new ActionGroup(x, djinnContext)).ToList();

			if (specification.Deployment.ActionGroups != null)
				this.deploymentActions = this.specification.Deployment.ActionGroups.Select( x => new ActionGroup(x, djinnContext)).ToList();
		}

		public String Identifier { get { return specification.BlueprintIdentifier; } }
		
		public String Description { get { return specification.Description; } }
		
		public CloudSecurityGroup Security { get { return securityGroup; } }
		
		// Defines what needs to be done to configure the server
		public List<ActionGroup> Configure
		{
			get { return configurationActions; }
		}
		
		// Defines what needs to be done to deploy code / dlls / apps to the server
		public List<ActionGroup> Deploy
		{
			get { return deploymentActions; }
		}
	}
}

