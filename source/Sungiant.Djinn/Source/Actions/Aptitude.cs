using System;
using System.Collections.Generic;
using Sungiant.Cloud;
using Sungiant.Core;
using ServiceStack.Text;
using System.Linq;

namespace Sungiant.Djinn
{
	public class Aptitude
		: Action<Specification.Aptitude>
	{
		public Aptitude (Specification.Aptitude specification, String djinnContext) 
			: base (specification, djinnContext) {}

		public override void Perform (ICloudProvider cloudProvider, ICloudDeployment cloudDeployment)
		{
			LogPerform ();

			if (this.Specification.RunUpdate)
			{
				cloudProvider.RunCommand (
					cloudDeployment, 
					"sudo apt-get -q -y update ");
			}

			if (this.Specification.Install != null)
			{
				cloudProvider.RunCommand (
					cloudDeployment, 
					"sudo apt-get -q -y install " + this.Specification.Install.Join (" "));
			}
		}
	}
}
