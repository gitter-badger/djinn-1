using System;
using System.Collections.Generic;

namespace Sungiant.Djinn
{
	public class MachineBlueprintSpecification
	{
		public String Description { get; set; }

		public String MachineBlueprintId { get; set; }

		public List<Int32> OpenPorts { get; set; }

		// Defines what needs to be done to configure the server
		public List<ActionGroup> Configure { get; set; }

		// Defines what needs to be done to deploy code / dlls / apps to the server
		public List<ActionGroup> Deploy { get; set; }
	}
}

