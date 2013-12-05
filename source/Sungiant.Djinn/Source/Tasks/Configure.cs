using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;
using Sungiant.Core;
using System.Text;
using Sungiant.Cloud;

namespace Sungiant.Djinn.Tasks
{
	public class Configure
		: Task
	{
		public String SpecificActionGroup { get; set; }

		public Configure (ICloudProvider cloudProvider, Deployment deployment)
			: base (TaskType.Configure, cloudProvider, deployment) {}

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

					var commandRunner = new CommandRunner (this.CloudProvider, cd);
					commandRunner.RunBatch (actionGroup);
				}
			}
		}
	}
}

