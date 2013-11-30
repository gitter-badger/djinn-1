using System;
using Sungiant.Cloud;
using Sungiant.Core;
using System.Collections.Generic;
using System.Reflection;

namespace Sungiant.Djinn
{
	public abstract class Action
	{
		public abstract void Perform(ICloudProvider cloudProvider, ICloudDeployment cloudDeployment);

		public static Action CreateFromSpecification(Specification.IAction specification, String djinnContext)
		{
			Type t = Type.GetType ("Sungiant.Djinn." + specification.Type + ", Sungiant.Djinn");

			Object o = Activator.CreateInstance (t, specification, djinnContext);

			return o as Action;
		}
	}

	public abstract class Action<T>
		: Action
	where T
		: Specification.IAction
	{
		readonly T specification;

		protected T Specification { get { return specification; } }

		readonly String djinnContext;

		public String DjinnContext
		{
			get { return djinnContext; }
		}

		protected Action (T specification, String djinnContext)
		{
			this.specification = specification;
			this.djinnContext = djinnContext;
		}

		protected void LogPerform()
		{
			Console.WriteLine("ACTION: " + specification.Type + " Action: " + Specification.Description + "\n");
		}
	}
}

