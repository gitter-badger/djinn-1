using System;
using System.Collections.Generic;
using System.Linq;
using Sungiant.Cloud;

namespace Sungiant.Djinn
{
	public class Deployment
	{
		Specification.Deployment Spec { get; set; }

		/// <summary>
		/// The directory from which local commands in this blueprint are relative to.
		/// </summary>
		readonly String localContext;
		
		public Deployment(
			Specification.Deployment spec, 
			Dictionary<String, Zone> deploymentGroups,
			Dictionary<String, Blueprint> machineBlueprints,
			String localContext)
		{
			this.Spec = spec;
			this.localContext = localContext;
			this.DeploymentGroup = deploymentGroups[spec.ZoneIdentifier];
			this.Blueprint = machineBlueprints[spec.BlueprintIdentifier];

			Identity = new CloudDeploymentIdentity()
			{
				IdentiferTags = new List<CloudDeploymentIdentifierTag>()
				{
					new CloudDeploymentIdentifierTag()
					{
						Name = "DeploymentId",
						Value = Identifier
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
								Name = "ZoneId",
								Value = DeploymentGroup.Identifier
							},
							new CloudDeploymentIdentifierTag()
							{
								Name = "BlueprintId",
								Value = Blueprint.Identifier
							}
						}
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
						Value = Blueprint.Description
					}
				}
			};
		}
		
		public Blueprint Blueprint { get; private set; }

		public Zone DeploymentGroup { get; private set; }

		public CloudDeploymentIdentity Identity { get; private set; }

		String Identifier { get { return DeploymentGroup.Identifier + " (" + Blueprint.Identifier + ")"; } }
		
		public Int32 HorizontalScale { get { return Spec.HorizontalScale; } }
		
		public Int32 VerticalScale { get { return Spec.VerticalScale; } }

		public String LocalContext { get { return localContext; }}
		
		public override String ToString ()
		{
			return String.Format ("[Deployment: Blueprint={0}, DeploymentGroup={1}, Identity={2}, HorizontalScale={3}, VerticalScale={4}]", Blueprint, DeploymentGroup, Identity, HorizontalScale, VerticalScale);
		}
	}
}

