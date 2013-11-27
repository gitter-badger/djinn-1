using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;
using Sungiant.Core;
using System.Text;
using Sungiant.Cloud;

namespace Sungiant.Djinn
{
	public class DjinnConfigureTask
		: DjinnTask
	{
		public String SpecificActionGroup { get; set; }

		public DjinnConfigureTask (ICloudProvider cloudProvider, Deployment deployment)
			: base (Task.Configure, cloudProvider, deployment) {}

		public override void Run()
		{
			var cd = CloudProvider.Describe (Deployment.Identity);
			
			if (cd != null)
			{
				foreach( var actionGroup in Deployment.Blueprint.Configure )
				{
					if(SpecificActionGroup != null && SpecificActionGroup != actionGroup.Identifier)
						continue;

					Console.WriteLine("Configure -> " + actionGroup.Description);

					foreach (Action action in actionGroup.Actions)
					{
						action.Perform (CloudProvider, cd);
					}
				}
			}
		}
	}
}

