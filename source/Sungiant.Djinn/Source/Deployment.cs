using System;
using System.Collections.Generic;
using System.Linq;
using Sungiant.Cloud;

namespace Sungiant.Djinn
{
	public class Deployment
	{
		readonly Int32 horizontalScale;
		readonly Int32 verticalScale;
		readonly Blueprint blueprint;
		readonly Zone zone;
		readonly CloudDeploymentIdentity identity;

		public Deployment(
			Specification.Deployment spec, 
			Zone zone,
			Blueprint blueprint)
		{
			this.zone = zone;
			this.blueprint = blueprint;
			this.horizontalScale = spec.HorizontalScale;
			this.verticalScale = spec.VerticalScale;

			identity = new CloudDeploymentIdentity()
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
								Value = Zone.Identifier
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
						Name = "MachineDescription",
						Value = Blueprint.Description
					}
				}
			};
		}

		public CloudDeploymentIdentity Identity { get { return identity; } }

		String Identifier { get { return Zone.Identifier + " (" + Blueprint.Identifier + ")"; } }

		public Blueprint Blueprint { get { return blueprint; } }

		public Zone Zone { get { return zone; } }

		public Int32 HorizontalScale { get { return horizontalScale; } }
		
		public Int32 VerticalScale { get { return verticalScale; } }

		public override String ToString ()
		{
			return String.Format ("[Deployment: Blueprint={0}, Zone={1}, Identity={2}, HorizontalScale={3}, VerticalScale={4}]", Blueprint, Zone, Identity, HorizontalScale, VerticalScale);
		}
	}
}

