using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;
using Sungiant.Core;
using System.Text;
using Sungiant.Cloud;

namespace Sungiant.Djinn
{
	public class DjinnProvisionTask
		: DjinnTask
	{
		public DjinnProvisionTask( ICloudProvider cloudProvider, Deployment deployment)
			: base(Task.Provision, cloudProvider, deployment) {}

		public override void Run()
		{
			var cp = CloudProvider.Describe(Deployment.Identity);
			
			if (cp == null)
			{
				CloudProvider.Launch(
					Deployment.Identity,
					Deployment.Blueprint.Security,
					Deployment.VerticalScale,
					Deployment.HorizontalScale
					);
			}
		}
	}
}

