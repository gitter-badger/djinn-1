using System;
using System.Collections.Generic;
using System.IO;
using ServiceStack.Text;
using Sungiant.Core;

namespace Sungiant.Djinn
{
	/// <summary>
	/// Represents all of the Djinn configuration file, updated at load time.
	/// </summary>
	public class Configuration
		: Singleton<Configuration>
	{
		static readonly String DjinnConfigurationFilePath;

		public ConfigFiles.DjinnFile DjinnFile { get; private set; }
		public ConfigFiles.InstallationFile DjinnInstallationFile { get; private set; }
		public ConfigFiles.SettingsFile DjinnSettingsFile { get; private set; }

		public Sungiant.Cloud.Aws.AwsCredentials DjinnAwsFile { get; private set; }
		public Sungiant.Cloud.Azure.AzureCredentials DjinnAzureFile { get; private set; }


		public ConfigFiles.DjinnFile.Workgroup ActiveWorkgroup
		{
			get { return DjinnFile.Workgroups[DjinnSettingsFile.ActiveWorkgroupIndex]; }
		}

		public DateTime InstallDateTime
		{ 
			get { return DateTimeHelper.FromUnixTime(DjinnInstallationFile.InstallTime); }
		}

		static Configuration()
		{
			DjinnConfigurationFilePath = System.Environment.GetEnvironmentVariable("HOME") + "/.djinn";
		}

		public void Load()
		{
			if (File.Exists (DjinnConfigurationFilePath))
			{
				DjinnFile = 
					File.ReadAllText (DjinnConfigurationFilePath)
						.FromJson<ConfigFiles.DjinnFile> ();

				if (File.Exists (DjinnConfigurationFilePath + ".aws"))
				{
					Instance.DjinnAwsFile = 
						File.ReadAllText (DjinnConfigurationFilePath + ".aws")
							.FromJson<Sungiant.Cloud.Aws.AwsCredentials> ();
				}

				if (File.Exists (DjinnConfigurationFilePath + ".azure"))
				{
					Instance.DjinnAzureFile = 
						File.ReadAllText (DjinnConfigurationFilePath + ".azure")
							.FromJson<Sungiant.Cloud.Azure.AzureCredentials> ();
				}

				if (File.Exists (DjinnConfigurationFilePath + ".installation"))
				{
					Instance.DjinnInstallationFile = 
						File.ReadAllText (DjinnConfigurationFilePath + ".installation")
							.FromJson<ConfigFiles.InstallationFile> ();
				}

				if (File.Exists (DjinnConfigurationFilePath + ".settings"))
				{
					Instance.DjinnSettingsFile = 
						File.ReadAllText (DjinnConfigurationFilePath + ".settings")
							.FromJson<ConfigFiles.SettingsFile> ();
				}

				if( DjinnAwsFile == null &&
					DjinnAzureFile == null &&
					DjinnInstallationFile == null &&
					DjinnSettingsFile == null )
				{
					throw new Exception(
						"You must add either ~/.djinn.aws or ~/.djinn.azure with appriopriate credentials");
				}
			}
			else
			{
				throw new Exception("Djinn's configuration file is missing, please reinstall.");
			}
		}
	}
}

