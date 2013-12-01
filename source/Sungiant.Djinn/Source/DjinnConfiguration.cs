using System;
using System.Collections.Generic;
using System.IO;
using ServiceStack.Text;
using Sungiant.Core;
using Sungiant.Djinn.Configuration;

namespace Sungiant.Djinn
{
	/// <summary>
	/// Represents all of the Djinn configuration file, updated at load time.
	/// </summary>
	public class DjinnConfiguration
		: Singleton<DjinnConfiguration>
	{
		static readonly String DjinnConfigurationFilePath;

		public DjinnFile DjinnFile { get; private set; }
		public DjinnInstallationFile DjinnInstallationFile { get; private set; }
		public DjinnSettingsFile DjinnSettingsFile { get; private set; }

		public Sungiant.Cloud.Aws.AwsCredentials DjinnAwsFile { get; private set; }
		public Sungiant.Cloud.Azure.AzureCredentials DjinnAzureFile { get; private set; }

		public DjinnFile.Workgroup ActiveWorkgroup
		{
			get { return DjinnFile.Workgroups[DjinnSettingsFile.ActiveWorkgroupIndex]; }
		}

		public DateTime InstallDateTime
		{ 
			get { return DateTimeHelper.FromUnixTime(DjinnInstallationFile.InstallTime); }
		}

		static DjinnConfiguration()
		{
			DjinnConfigurationFilePath = Environment.GetEnvironmentVariable("HOME") + "/.djinn";
		}

		public void Load()
		{
			if (File.Exists (DjinnConfigurationFilePath))
			{
                String rawDjinnFile = File.ReadAllText (DjinnConfigurationFilePath);
				DjinnFile = rawDjinnFile.FromJson<DjinnFile> ();

				if (File.Exists (DjinnConfigurationFilePath + ".aws"))
				{
                    String rawDjinnAwsFile = File.ReadAllText (DjinnConfigurationFilePath + ".aws");
					DjinnConfiguration.Instance.DjinnAwsFile = 
						rawDjinnAwsFile.FromJson<Sungiant.Cloud.Aws.AwsCredentials> ();
				}

				if (File.Exists (DjinnConfigurationFilePath + ".azure"))
				{
                    String rawDjinnAzureFile = File.ReadAllText (DjinnConfigurationFilePath + ".azure");
					DjinnConfiguration.Instance.DjinnAzureFile = 
						rawDjinnAzureFile.FromJson<Sungiant.Cloud.Azure.AzureCredentials> ();
				}

				if (File.Exists (DjinnConfigurationFilePath + ".installation"))
				{
                    String rawDjinnInstallationFile = File.ReadAllText (DjinnConfigurationFilePath + ".installation");
					DjinnConfiguration.Instance.DjinnInstallationFile = 
						rawDjinnInstallationFile.FromJson<DjinnInstallationFile> ();
				}

				if (File.Exists (DjinnConfigurationFilePath + ".settings"))
				{
                    String rawDjinnSettingsFile = File.ReadAllText (DjinnConfigurationFilePath + ".settings");
					DjinnConfiguration.Instance.DjinnSettingsFile = 
						rawDjinnSettingsFile.FromJson<DjinnSettingsFile> ();
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

