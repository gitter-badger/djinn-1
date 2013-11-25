using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using ServiceStack.Text;

using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.DynamoDB;
using Amazon.DynamoDB.Model;
using Amazon.SQS;

using Sungiant.Core;

namespace Sungiant.Cloud.Aws
{
	public class Aws
		: ICloudProvider
	{
		AwsCredentials AwsCredentials { get; set; }
		
		public Aws(AwsCredentials credentials)
		{
			AwsCredentials = credentials;
		}

		#region ICloudProvider

		public String PrivateKeyPath { get { return AwsCredentials.PrivateKeyPath; } }

		public String User { get { return "ubuntu"; } }

		public void PrintStatus()
		{
			using(var client = AWSClientFactory.CreateAmazonEC2Client(AwsCredentials.AccessKeyId, AwsCredentials.SecretKey))
			{
				AwsEc2SecurityGroupHelper.PrintStatus(client);
				Console.WriteLine("");
				AwsEc2Helper.PrintStatus(client);

			}
		}


		public ICloudDeployment Describe(CloudDeploymentIdentity identifier)
		{
			using(var client = AWSClientFactory.CreateAmazonEC2Client(AwsCredentials.AccessKeyId, AwsCredentials.SecretKey))
			{
				var result = AwsEc2Helper.Describe(client, identifier);
				
				return result;
			}
		}

		public void Launch(
			CloudDeploymentIdentity instanceIdentifier,
			CloudSecurityGroup securityGroup,
			int verticleScale,
			int horizontalScale
			)
		{
			using(var client = AWSClientFactory.CreateAmazonEC2Client(AwsCredentials.AccessKeyId, AwsCredentials.SecretKey))
			{
				if( AwsEc2SecurityGroupHelper.Exists(client, securityGroup.Name) )
				{
					if( !AwsEc2SecurityGroupHelper.Matches(client, securityGroup) )
					{
						// problem, the security group is in place, but not as we expect
						// ask the user what to do

						throw new NotImplementedException();
					}
				}
				else
				{
					AwsEc2SecurityGroupHelper.Create(client, securityGroup);
				}

				// now securityGroupName is ready
				
				AwsEc2Helper.GetCloudDeployment(
					client,
					instanceIdentifier,
					AwsCredentials.PrivateKeyName,
					securityGroup.Name,
					horizontalScale,
					verticleScale);
			}

		}

		public void RunCommands(ICloudDeployment deployment, String[] commands, Boolean ignoreFailures)
		{
			foreach (var endpoint in deployment.Endpoints)
			{
				commands = commands
					.Where (x => !string.IsNullOrEmpty (x))
					.Select (x => x.Replace ("\"", "\\\""))
					.ToArray ();

				string command = string.Format ("\"{0}\"", string.Join ("; ", commands));
				;

				Int32 exitCode = ProcessHelper.Run (
					new String[]
					{
						"ssh",
						"-o StrictHostKeyChecking=no",
						"-i", 
						PrivateKeyPath,
						string.Format ("{0}@{1}", User, endpoint),
						command 
					}.Join (" "),
					Console.WriteLine);

				if (exitCode != 0 && !ignoreFailures)
				{
					throw new Exception ("Exited with code " + exitCode);
				}
			}
		}

		public void RunCommand(ICloudDeployment deployment, String command, Boolean ignoreFailure)
		{
			RunCommands (deployment, new String[]{ command }, ignoreFailure);
		}

		public void Destroy(CloudDeploymentIdentity instanceIdentifier)
		{
			using(var client = AWSClientFactory.CreateAmazonEC2Client(AwsCredentials.AccessKeyId, AwsCredentials.SecretKey))
			{
				var awsCd = AwsEc2Helper.Describe(client, instanceIdentifier);
				
				var response = client.TerminateInstances(
					new TerminateInstancesRequest()
					{
						InstanceId = awsCd.RunningInstances.Select(x => x.InstanceId).ToList()
					}
				);
					
				response.TerminateInstancesResult.PrintDump();

//				DestroySecurityGroup(machineBlueprintId);
			}
		}

		#endregion

	}
}

