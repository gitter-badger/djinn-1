using System;

namespace Sungiant.Djinn.Specification
{
	public class MdToolBuild
		: IAction
	{
		// Base class data
		public String Type { get; set; }
		public String Description { get; set; }

		// MdToolBuildSpecification data
		public String Configuration { get; set; }
		public String SolutionPath { get; set; }
		public Boolean Verbose { get; set; } 
		public Boolean IsContextRemote { get; set; }
	}
}

