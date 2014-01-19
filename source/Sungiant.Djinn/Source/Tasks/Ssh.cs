using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;
using System.Text;
using Sungiant.Cloud;

namespace Sungiant.Djinn.Tasks
{
	public class Ssh
		: Task
	{
		public Ssh (ICloudProvider cloudProvider, Deployment deployment)
			: base (TaskType.Ssh, cloudProvider, deployment) {}

		public override void Run(Boolean dryRun)
		{
			var cd = CloudProvider.Describe (Deployment.Identity);
			
			if (cd != null)
			{
				int machineIndex = 0;
				
				if (cd.Endpoints.Count > 1)
				{
					Console.WriteLine ("Specify the machine index to ssh into [0-" + cd.Endpoints.Count + "]");
					machineIndex = int.Parse( Console.ReadLine());
				}
			
				Int32 exitCode = ProcessHelper.Run(
					new string[]
					{
						"ssh",
						"-i", 
						CloudProvider.PrivateKeyPath,
						"-o StrictHostKeyChecking=no",
						CloudProvider.User + "@" + cd.Endpoints[machineIndex]
					}.Join(" ")
				);

				if (exitCode != 0)
				{
					if (exitCode == 255)
						Console.WriteLine ("Session closed");
					else
						throw new Exception ("Exited with code " + exitCode);
				}
			}
		}
	}
}

