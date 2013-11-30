using System;
using System.Collections.Generic;
using System.IO;
using ServiceStack.Text;
using Sungiant.Core;

namespace Sungiant.Djinn
{
	/// <summary>
	/// 
	/// </summary>
	public class DjinnInstallationFile
	{
		/// <summary>
		/// When was Djinn last installed.  (todo: this should live in a different file)
		/// </summary>
		public Int32 InstallTime { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class DjinnSettingsFile
	{
		public int ActiveWorkgroupIndex { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class DjinnFile
	{
		/// <summary>
		/// 
		/// </summary>
		public class ProjectConfiguration
		{
			public String ProjectIdentifier { get; set; }
			public String DjinnDirectory { get; set; }

			public String BlueprintsDirectory { get { return Path.Combine (DjinnDirectory, "blueprints"); } }
			public String ZonesDirectory { get { return Path.Combine (DjinnDirectory, "zones"); } }
		}

		/// <summary>
		/// A workgroup represents a group of 
		/// </summary>
		public class Workgroup
		{
			public String WorkgroupIdentifier { get; set; }
			public List<ProjectConfiguration> ProjectConfigurations { get; set; }
		}

		/// <summary>
		/// 
		/// </summary>
		public List<Workgroup> Workgroups { get; set; }
	}

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
				DjinnFile = 
					File.ReadAllText (DjinnConfigurationFilePath)
						.FromJson<DjinnFile> ();

				if (File.Exists (DjinnConfigurationFilePath + ".aws"))
				{
					DjinnConfiguration.Instance.DjinnAwsFile = 
						File.ReadAllText (DjinnConfigurationFilePath + ".aws")
							.FromJson<Sungiant.Cloud.Aws.AwsCredentials> ();
				}

				if (File.Exists (DjinnConfigurationFilePath + ".azure"))
				{
					DjinnConfiguration.Instance.DjinnAzureFile = 
						File.ReadAllText (DjinnConfigurationFilePath + ".azure")
							.FromJson<Sungiant.Cloud.Azure.AzureCredentials> ();
				}

				if (File.Exists (DjinnConfigurationFilePath + ".installation"))
				{
					DjinnConfiguration.Instance.DjinnInstallationFile = 
						File.ReadAllText (DjinnConfigurationFilePath + ".installation")
							.FromJson<DjinnInstallationFile> ();
				}

				if (File.Exists (DjinnConfigurationFilePath + ".settings"))
				{
					DjinnConfiguration.Instance.DjinnSettingsFile = 
						File.ReadAllText (DjinnConfigurationFilePath + ".settings")
							.FromJson<DjinnSettingsFile> ();
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

