using System;

namespace Sungiant.Djinn.Specification
{
	public class ActionGroup
	{
		public String ActionGroupIdentifier { get; set; }
		public String Description { get; set; }
		public IAction[] Actions { get; set; }
	}
}

