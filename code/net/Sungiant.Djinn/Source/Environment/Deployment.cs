using System;
using System.Collections.Generic;
using System.Linq;
using Sungiant.Cloud;

namespace Sungiant.Djinn
{
	public class Deployment
	{
		DeploymentSpecification Spec { get; set; }
		
		public Deployment(
			DeploymentSpecification spec, 
			Dictionary<String, DeploymentGroup> deploymentGroups,
			Dictionary<String, MachineBlueprint> machineBlueprints)
		{
			this.Spec = spec;
			this.DeploymentGroup = deploymentGroups[spec.DeploymentGroupId];
			this.MachineBlueprint = machineBlueprints[spec.MachineBlueprintId];

			Identity = new CloudDeploymentIdentity()
			{
				IdentiferTags = new List<CloudDeploymentIdentifierTag>()
				{
					new CloudDeploymentIdentifierTag()
					{
						Name = "DeploymentId",
						Value = Id,
					},
				},
				IdentiferTagGroups = new List<CloudDeploymentIdentifierTagGroup>()
				{
					new CloudDeploymentIdentifierTagGroup()
					{
						PartialIdentiferTags = new List<CloudDeploymentIdentifierTag>()
						{
							new CloudDeploymentIdentifierTag()
							{
								Name = "DeploymentGroupId",
								Value = DeploymentGroup.Id,
							},
							new CloudDeploymentIdentifierTag()
							{
								Name = "MachineBlueprintId",
								Value = MachineBlueprint.Id,
							},
						},
					}
				},
				InformationalTags = new List<CloudDeploymentIdentifierTag>()
				{
					new CloudDeploymentIdentifierTag()
					{
						Name = "DeploymentDescription",
						Value = DeploymentGroup.Description
					},
					new CloudDeploymentIdentifierTag()
					{
						Name = "MachineDescription",
						Value = MachineBlueprint.Description
					},
				}
			};
		}
		
		public MachineBlueprint MachineBlueprint { get; private set; }

		public DeploymentGroup DeploymentGroup { get; private set; }

		public CloudDeploymentIdentity Identity { get; private set; }

		String Id { get { return DeploymentGroup.Id + " (" + MachineBlueprint.Id + ")"; } }
		
		public Int32 HorizontalScale { get { return Spec.HorizontalScale; } }
		
		public Int32 VerticalScale { get { return Spec.VerticalScale; } }
		
		public override string ToString ()
		{
			return string.Format ("[Deployment: MachineBlueprint={0}, DeploymentGroup={1}, Identity={2}, HorizontalScale={3}, VerticalScale={4}]", MachineBlueprint, DeploymentGroup, Identity, HorizontalScale, VerticalScale);
		}
	}
}

