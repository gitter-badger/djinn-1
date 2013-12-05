using System;
using System.IO;
using System.Collections.Generic;

namespace Sungiant.Djinn.ConfigFiles
{
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

}

