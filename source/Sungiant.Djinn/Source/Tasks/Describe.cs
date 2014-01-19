// ┌────────────────────────────────────────────────────────────────────────┐ \\
using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;
using System.Text;
using Sungiant.Cloud;

namespace Sungiant.Djinn.Tasks
{
	public class Describe
		: Task
	{
		public Describe (ICloudProvider cloudProvider, Deployment deployment)
			: base (TaskType.Describe, cloudProvider, deployment) {}

		public override void Run(Boolean dryRun)
		{
			var cd = CloudProvider.Describe (Deployment.Identity);
			
			if (cd != null)
			{
				Console.WriteLine(cd.Endpoints.Join(", "));
			}
			else
			{
				Console.WriteLine("Deployment offline");
			}
		}
	}
}

