using System;
using System.Collections.Generic;

namespace Sungiant.Cloud.Azure
{
	public class Azure
		: ICloudProvider
	{
		AzureCredentials AzureCredentials { get; set; }

		public String PrivateKeyPath { get { throw new NotImplementedException(); } }

		public String User { get { return "sungiant"; } }


		public void RunCommand(ICloudDeployment deployment, String commands)
		{
			throw new NotImplementedException ();
		}

		public void RunCommands(ICloudDeployment deployment, String[] commands)
		{
			throw new NotImplementedException ();
		}

		public Azure(AzureCredentials credentials)
		{
			AzureCredentials = credentials;
		}

		public List<string> GetEndpoints (CloudDeploymentIdentity instanceIdentifier)
		{
			throw new NotImplementedException ();
		}
		
		public void Launch(
			CloudDeploymentIdentity instanceIdentifier,
			CloudSecurityGroup securityGroup,
			Int32 verticleScale,
			Int32 horizontalScale
			)
		{
			throw new NotImplementedException ();
		}
		
		public void Destroy(CloudDeploymentIdentity instanceIdentifier)
		{
			throw new NotImplementedException ();
		}

		public void PrintStatus()
		{
			throw new NotImplementedException ();
		}

		public ICloudDeployment Describe(CloudDeploymentIdentity identifier)
		{
			throw new NotImplementedException ();
		}
	}
}

