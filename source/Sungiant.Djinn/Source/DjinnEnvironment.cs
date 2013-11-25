using System;
using System.Collections.Generic;
using System.Linq;

namespace Sungiant.Djinn
{
	public class DjinnEnvironment
	{
		DjinnEnvironmentSetupData Spec { get; set; }

		public DjinnEnvironment(DjinnEnvironmentSetupData spec)
		{
			Spec = spec;

			var blueprints = spec.Projects
				.SelectMany (x => x.BlueprintSpecifications.Select (y => new Blueprint (y)))
				.ToDictionary (x => x.Id, y => y);

			var zones = spec.Projects
				.SelectMany (x => x.ZoneSpecifications.Select (y => new Zone (y)))
				.ToDictionary (x => x.Id, y => y);

			Deployments = spec.Projects
				.SelectMany (x => x.DeploymentSpecifications.Select (y => new Deployment(y, zones, blueprints, x.LocalContext)))
				.ToList();

		}

		/// <summary>
		/// All Deployments in the current workgroup.
		/// </summary>
		public List<Deployment> Deployments { get; private set; }
	}
}

