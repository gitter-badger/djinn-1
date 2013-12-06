using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;
using Sungiant.Core;
using System.Text;
using Sungiant.Cloud;

namespace Sungiant.Djinn.Tasks
{
	public class RunActions
		: Task
	{
		public String SpecificActionGroup { get; set; }

		public RunActions (ICloudProvider cloudProvider, Deployment deployment)
			: base (TaskType.RunActions, cloudProvider, deployment) {}

		public override void Run(Boolean dryRun)
		{
			var cd = CloudProvider.Describe (Deployment.Identity);
			
			if (cd != null)
			{
				foreach( var actionGroup in Deployment.Blueprint.Configure )
				{
					if(SpecificActionGroup != null && SpecificActionGroup != actionGroup.Identifier)
						continue;

					Console.WriteLine("Configure -> " + actionGroup.Description);

					var commandRunner = new CommandRunner (this.CloudProvider, cd, dryRun);
					commandRunner.RunBatch (actionGroup);
				}
			}
		}
	}
}

