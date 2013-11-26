using System;
using System.Collections.Generic;
using System.Linq;

namespace Sungiant.Djinn
{
	public class DjinnEnvironment
	{
		readonly DjinnEnvironmentSetupData setupData;

		readonly List<Deployment> deployments;

		public DjinnEnvironment(DjinnEnvironmentSetupData specification)
		{
			this.setupData = specification;

			var blueprints = this.setupData.Projects
				.SelectMany (x => x.BlueprintSpecifications.Select (y => new Blueprint (y, x.LocalContext)))
				.ToDictionary (x => x.Identifier, y => y);

			var zones = this.setupData.Projects
				.SelectMany (x => x.ZoneSpecifications.Select (y => new Zone (y)))
				.ToDictionary (x => x.Identifier, y => y);

			deployments = this.setupData.Projects
				.SelectMany (x => x.DeploymentSpecifications.Select (y => new Deployment(y, zones, blueprints, x.LocalContext)))
				.ToList ();

		}

		/// <summary>
		/// All Deployments in the current workgroup.
		/// </summary>
		public List<Deployment> Deployments
		{
			get { return deployments; }
		}
	}
}

