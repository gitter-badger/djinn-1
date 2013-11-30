using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Sungiant.Djinn
{
	public class Project
	{
		readonly ProjectSetupData setupData;
		readonly List<Deployment> deployments = new List<Deployment>();

		public String LocalContext { get { return this.setupData.LocalContext; } }

		public List<Deployment> Deployments { get { return this.deployments; } }

		public Project(ProjectSetupData setupData)
		{
			this.setupData = setupData;

			Dictionary<String, Blueprint> blueprints = this.setupData.BlueprintSpecifications
				.Select (y => new Blueprint (y, Path.Combine (LocalContext, "blueprints")))
				.ToDictionary (x => x.Identifier, y => y);

			List< Zone> zones = this.setupData.ZoneSpecifications
				.Select (y => new Zone (y, blueprints))
				.ToList ();

			deployments = zones.SelectMany (x => x.Deployments).ToList ();
		}
	}
}

