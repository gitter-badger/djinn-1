using System;

namespace Sungiant.Djinn.Specification
{
	public class AptitudeInstallation
		: IAction
	{
		// Base class data
		public String Type { get; set; }
		public String Description { get; set; }

		// AptitudeInstallation data
		public String[] PackageNames { get; set; }
	}
}

