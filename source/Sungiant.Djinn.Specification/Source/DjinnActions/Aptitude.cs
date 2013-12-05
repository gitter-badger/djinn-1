using System;

namespace Sungiant.Djinn.Specification
{
	public class Aptitude
		: IAction
	{
		// Base class data
		public String Type { get; set; }
		public String Description { get; set; }

		public Boolean RunUpdate { get; set; }

		// AptitudeInstallation data
		public String[] Install { get; set; }
	}
}

