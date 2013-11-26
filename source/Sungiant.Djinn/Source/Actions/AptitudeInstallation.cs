using System;
using System.Collections.Generic;
using Sungiant.Cloud;
using Sungiant.Core;
using ServiceStack.Text;
using System.Linq;

namespace Sungiant.Djinn
{
	public class AptitudeInstallation
		: Action<Specification.AptitudeInstallation>
	{
		public AptitudeInstallation (Specification.AptitudeInstallation specification, String djinnContext) 
			: base (specification, djinnContext) {}

		public override void Perform (ICloudProvider cloudProvider, ICloudDeployment cloudDeployment)
		{
			LogPerform ();

			cloudProvider.RunCommand (
				cloudDeployment, 
				"sudo apt-get -q -y install " + this.Specification.PackageNames.Join(" "));
		}
	}
}
