using System;
using System.Collections.Generic;
using System.IO;
using ServiceStack.Text;
using Sungiant.Core;

namespace Sungiant.Djinn
{
	public class DjinnConfiguration
		: Singleton<DjinnConfiguration>
	{	

		public Int32 InstallTime { get; set; }
		public DateTime InstallDateTime { get { return DateTimeHelper.FromUnixTime(InstallTime); } }

		public Sungiant.Cloud.Aws.AwsCredentials AwsCredentials { get; private set; }
		public Sungiant.Cloud.Azure.AzureCredentials AzureCredentials { get; private set; }

		public class Workgroup
		{
			public String RepoDirectory { get; set; }
			public String MachineBlueprintSpecificationsDirectory { get; set; }
			public String DeploymentSpecificationsDirectory { get; set; }
			public String DeploymentGroupSpecificationsDirectory { get; set; }
		}

		public List<Workgroup> Workgroups { get; set; }

		public int ActiveWorkgroupIndex { get; set; }

		public Workgroup ActiveWorkgroup
		{
			get
			{
				return Workgroups[ActiveWorkgroupIndex];
			}
		}

		static readonly String DjinnConfigurationFilePath;

		static DjinnConfiguration()
		{
			DjinnConfigurationFilePath = Environment.GetEnvironmentVariable("HOME") + "/.djinn";
		}

		public static DjinnConfiguration Load()
		{
			if (File.Exists (DjinnConfigurationFilePath))
			{
				String content = File.ReadAllText (DjinnConfigurationFilePath);

				DjinnConfiguration result = content.FromJson<DjinnConfiguration> ();

				if (File.Exists (DjinnConfigurationFilePath + ".aws"))
				{
					result.AwsCredentials = 
						File.ReadAllText (DjinnConfigurationFilePath + ".aws")
							.FromJson<Sungiant.Cloud.Aws.AwsCredentials> ();	
				}
				
				if (File.Exists (DjinnConfigurationFilePath + ".azure"))
				{
					result.AzureCredentials = 
						File.ReadAllText (DjinnConfigurationFilePath + ".azure")
							.FromJson<Sungiant.Cloud.Azure.AzureCredentials> ();	
					
				}

				if( result.AwsCredentials == null && result.AzureCredentials == null )
				{
					throw new Exception("You must add either ~/.djinn.aws or ~/.djinn.azure with appriopriate credentials");
				}

				return result;
			}
			else
			{
				throw new Exception("Djinn's configuration file is missing, please reinstall.");
			}
		}
	}
}

