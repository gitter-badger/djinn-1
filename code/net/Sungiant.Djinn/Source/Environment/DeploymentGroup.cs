using System;

namespace Sungiant.Djinn
{
	
	public class DeploymentGroup
	{
		DeploymentGroupSpecification spec;
		
		public DeploymentGroup(DeploymentGroupSpecification spec)
		{
			this.spec = spec;
		}
		
		public String Id { get { return spec.DeploymentGroupId; } }
		
		public String Description { get { return spec.Description; } }
		
	}
}

