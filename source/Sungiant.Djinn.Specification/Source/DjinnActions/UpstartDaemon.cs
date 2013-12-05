using System;

namespace Sungiant.Djinn.Specification
{
	public class UpstartDaemon
		: IAction
	{
		// Base class data
		public String Type { get; set; }
		public String Description { get; set; }

		// UpstartDaemonSpecification data
		public String DaemonName { get; set; }
		public String[] DaemonCommands { get; set; }
	}
}

