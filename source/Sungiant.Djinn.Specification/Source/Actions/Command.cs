using System;

namespace Sungiant.Djinn.Specification
{
	public class Command
		: IAction
	{
		// Base class data
		public String Type { get; set; }
		public String Description { get; set; }

		// CommandSpecification data
		public Boolean IsContextRemote { get; set; }
		public String Value { get; set; }
		public Boolean IgnoreFailure { get; set; }
	}
}

