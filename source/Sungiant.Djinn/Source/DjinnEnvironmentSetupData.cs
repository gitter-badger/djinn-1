using System;
using System.Collections.Generic;

namespace Sungiant.Djinn
{
	public class DjinnEnvironmentSetupData
	{
		public void AddProject (
			String localContext,
			List<BlueprintSpecification> blueprintSpecifications,
			List<DeploymentSpecification> deploymentSpecifications,
			List<ZoneSpecification> zoneSpecifications)
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

			public List<BlueprintSpecification> BlueprintSpecifications { get; set; }
			public List<DeploymentSpecification> DeploymentSpecifications { get; set; }
			public List<ZoneSpecification> ZoneSpecifications { get; set; }
		}

		readonly List<Project> projects = new List<Project>();

		public List<Project> Projects { get { return projects; } }
	}
}

