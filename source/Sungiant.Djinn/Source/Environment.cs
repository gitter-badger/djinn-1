using System;
using System.Collections.Generic;
using System.Linq;

namespace Sungiant.Djinn
{
	public class Environment
	{
		readonly EnvironmentSetupData setupData;

		readonly List<Project> projects;

		public Environment(EnvironmentSetupData specification)
		{
			this.setupData = specification;

			this.projects = this.setupData.ProjectSetupDatas
				.Select (x => new Project (x))
				.ToList ();
		}

		public List<Project> Projects
		{
			get { return this.projects; }
		}
	}
}

