using System;

namespace Sungiant.Djinn.Specification
{
	public class XBuild
		: IAction
	{
		// Base class data
		public String Type { get; set; }
		public String Description { get; set; }

		// XbuildSpecification data
		public String Configuration { get; set; }
		public String ProjectPath { get; set; }
		public String Verbosity  { get; set; }
		public Boolean IsContextRemote { get; set; }
	}
}

