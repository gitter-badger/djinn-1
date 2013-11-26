using System;
using System.Collections.Generic;

namespace Sungiant.Djinn
{
	public class DjinnEnvironmentSetupData
	{
		public void AddProject (
			String localContext,
			List<Specification.Blueprint> blueprintSpecifications,
			List<Specification.Deployment> deploymentSpecifications,
			List<Specification.Zone> zoneSpecifications)
		{
			projects.Add (
				new Project()
				{
					LocalContext = localContext,
					BlueprintSpecifications = blueprintSpecifications,
					DeploymentSpecifications = deploymentSpecifications,
					ZoneSpecifications = zoneSpecifications
				});
		}

		public class Project
		{
			public String LocalContext { get; set; }

			public List<Specification.Blueprint> BlueprintSpecifications { get; set; }
			public List<Specification.Deployment> DeploymentSpecifications { get; set; }
			public List<Specification.Zone> ZoneSpecifications { get; set; }
		}

		readonly List<Project> projects = new List<Project>();

		public List<Project> Projects { get { return projects; } }
	}
}

