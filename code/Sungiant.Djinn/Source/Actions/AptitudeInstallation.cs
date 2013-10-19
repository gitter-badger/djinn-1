using System;
using System.Collections.Generic;
using Sungiant.Cloud;
using Sungiant.Core;
using ServiceStack.Text;

namespace Sungiant.Djinn
{
	public class AptitudeInstallation
		: Action
	{
		public AptitudeInstallation(String description) 
			: base(description) { }

		public List<String> PackageNames { get; set; }

		public override void Perform(ICloudProvider cloudProvider, ICloudDeployment cloudDeployment)
		{
			LogPerform();

			cloudProvider.RunCommand(
				cloudDeployment, 
				"sudo apt-get",
				"-q -y install " + PackageNames.Join(" ")
			);
		}
	}
}
