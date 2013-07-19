using System;
using Sungiant.Cloud;
using Sungiant.Core;
using System.Collections.Generic;

namespace Sungiant.Djinn
{
	public abstract class Action
	{
		public ActionType Type { get; private set; } 

		public string Description { get; private set; } 

		protected Action (ActionType type, String description)
		{
			Type = type;
			Description = description;
		}



		protected void LogPerform()
		{
			Console.WriteLine(" --> Performing " + Type + " Action: " + Description);
		}

		public abstract void Perform(ICloudProvider cloud, ICloudDeployment deployment);
	}
}

