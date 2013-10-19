using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;
using Sungiant.Core;
using System.Text;
using Sungiant.Cloud;

namespace Sungiant.Djinn
{
	public class DjinnSshTask
		: DjinnTask
	{
		public DjinnSshTask( ICloudProvider cloudProvider, Deployment deployment)
			: base(Task.Ssh, cloudProvider, deployment) {}

		public override void Run()
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
			
				ProcessHelper.Run(
					"ssh", 
					new string[]
					{
						"-i", 
						CloudProvider.PrivateKeyPath,
						CloudProvider.User + "@" + cd.Endpoints[machineIndex]
					}.Join(" ")
				);
			}
		}
	}
}

