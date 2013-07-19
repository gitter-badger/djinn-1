using System;
using System.Collections.Generic;
using Amazon.EC2.Model;
using System.Linq;

namespace Sungiant.Cloud.Aws
{
	internal class AwsCloudDeployment
		: ICloudDeployment
	{
		internal AwsCloudDeployment(CloudDeploymentIdentity identity, List<RunningInstance> instances)
		{
			RunningInstances = instances;
			Identifier = identity;
			
		}
		
		
		internal List<RunningInstance> RunningInstances { get; private set; }
		
		public Int32 VerticleScale 
		{ 
			get
			{
				throw new NotImplementedException();
		 	}
		}
		
		public Int32 HorizontalScale 
		{ 
			get
			{
				return RunningInstances.Count;
		 	}
		}
		
		public List<String> Endpoints
		{ 
			get
			{
				return RunningInstances.Select(x => x.IpAddress).ToList();
		 	}
		}
		
		public CloudDeploymentIdentity Identifier { get; private set; }
		
		public CloudDeploymentStatus Status
		{ 
			get
			{
				throw new NotImplementedException();
		 	}
		}
	}
}

