using System;
using System.Collections.Generic;
using System.Linq;

namespace Sungiant.Djinn
{
	public class ActionGroup
	{
		readonly Specification.ActionGroup specification;
		readonly List<Action> actions;

		public String Identifier
		{
			get { return specification.ActionGroupIdentifier; }
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

			if (specification.Actions != null)
				this.actions = specification.Actions.Select (x => Action.CreateFromSpecification (x, djinnContext)).ToList ();
			else
				this.actions = new List<Action> ();
		}
	}
}

