using System;
using Sungiant.Cloud;
using Sungiant.Core;
using System.Collections.Generic;

namespace Sungiant.Djinn
{
	public class ActionGroup
	{
		public String Name { get; set; }
		public String Description { get; set; }
		public List<Action> Actions { get; set; }
	}
}

