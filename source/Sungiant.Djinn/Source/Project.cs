using System;
using System.Collections.Generic;
using System.Linq;

namespace Sungiant.Djinn
{
	public class Project
	{
		readonly ProjectSetupData setupData;
		readonly List<Deployment> deployments;

		public String LocalContext { get { return this.setupData.LocalContext; } }

		public List<Deployment> Deployments { get { return this.deployments; } }

		public Project(ProjectSetupData setupData)
		{
			this.setupData = setupData;

			var blueprints = this.setupData.BlueprintSpecifications
				.Select (y => new Blueprint (y, LocalContext))
				.ToDictionary (x => x.Identifier, y => y);

			var zones = this.setupData.ZoneSpecifications
				.Select (y => new Zone (y))
				.ToDictionary (x => x.Identifier, y => y);

			deployments = this.setupData.DeploymentSpecifications
				.Select (x => new Deployment (x, zones, blueprints, LocalContext))
				.ToList ();
		}
	}
}

