using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;
using Sungiant.Core;
using System.Text;
using Sungiant.Cloud;

namespace Sungiant.Djinn.Tasks
{
	public class Deploy
		: Task
	{
		public String SpecificActionGroup { get; set; }

		public Deploy (ICloudProvider cloudProvider, Deployment deployment)
			: base (TaskType.Deploy, cloudProvider, deployment) {}

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

					var commandRunner = new CommandRunner (this.CloudProvider, cd);
					commandRunner.RunBatch (actionGroup);
				}
			}
		}
	}
}

