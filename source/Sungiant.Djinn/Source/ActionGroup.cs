using System;
using Sungiant.Cloud;
using Sungiant.Core;
using System.Collections.Generic;
using System.Linq;

namespace Sungiant.Djinn
{
	public class ActionGroup
	{
		readonly Specification.ActionGroup specification;
		readonly List<Action> actions;

		public String Name
		{
			get { return specification.Name; }
		}

		public String Description
		{
			get { return specification.Description; }
		}

		public List<Action> Actions
		{
			get { return actions; }
		}

		public ActionGroup(Specification.ActionGroup specification, String djinnContext)
		{
			this.specification = specification;
			this.actions = specification.Actions.Select (x => Action.CreateFromSpecification (x, djinnContext)).ToList();
		}
	}
}

