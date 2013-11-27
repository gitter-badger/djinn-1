using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;
using Sungiant.Core;
using System.Text;
using Sungiant.Cloud;

namespace Sungiant.Djinn
{
	public class DjinnDeployTask
		: DjinnTask
	{
		public String SpecificActionGroup { get; set; }

		public DjinnDeployTask (ICloudProvider cloudProvider, Deployment deployment)
			: base (Task.Deploy, cloudProvider, deployment) {}

		public override void Run()
		{
			var cd = CloudProvider.Describe (Deployment.Identity);
			
			if (cd != null)
			{
				foreach( var actionGroup in Deployment.Blueprint.Deploy )
				{
					if(SpecificActionGroup != null && SpecificActionGroup != actionGroup.Identifier)
						continue;

					Console.WriteLine("Deploying -> " + actionGroup.Description);

					foreach( var action in actionGroup.Actions )
					{
						action.Perform(CloudProvider, cd);
					}
				}
			}
		}
	}
}

