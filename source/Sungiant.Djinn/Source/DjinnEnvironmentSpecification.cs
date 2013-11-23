using System;
using System.Collections.Generic;

namespace Sungiant.Djinn
{
	public class DjinnEnvironmentSpecification
	{
		public List<MachineBlueprintSpecification> MachineBlueprintSpecifications { get; set; }
		public List<DeploymentSpecification> DeploymentSpecifications { get; set; }
		public List<DeploymentGroupSpecification> DeploymentGroupSpecifications { get; set; }

	}
}

