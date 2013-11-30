using System;
using System.Collections.Generic;

namespace Sungiant.Djinn
{
	public class EnvironmentSetupData
	{
		public EnvironmentSetupData (String workgroupName)
		{
			this.workgroupName = workgroupName;
		}

		readonly String workgroupName;
		readonly List<ProjectSetupData> projects = new List<ProjectSetupData>();

		public List<ProjectSetupData> ProjectSetupDatas { get { return projects; } }

		public String WorkgroupName { get { return this.workgroupName; } }

		public void AddProject (
			String localContext,
			List<Specification.Blueprint> blueprintSpecifications,
			List<Specification.Zone> zoneSpecifications)
		{
			projects.Add (
				new ProjectSetupData()
				{
					LocalContext = localContext,
					BlueprintSpecifications = blueprintSpecifications,
					ZoneSpecifications = zoneSpecifications
				});
		}
	}
}

