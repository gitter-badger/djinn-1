using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;
using Sungiant.Core;
using System.Text;
using Sungiant.Cloud;

namespace Sungiant.Djinn.Tasks
{
	public class Destroy
		: Task
	{
		public Destroy (ICloudProvider cloudProvider, Deployment deployment)
			: base (TaskType.Destroy, cloudProvider, deployment) {}

		public override void Run()
		{
			
			var cd = CloudProvider.Describe (Deployment.Identity);
			
			if (cd != null)
			{
				CloudProvider.Destroy(Deployment.Identity);
			}
		}
	}
}

