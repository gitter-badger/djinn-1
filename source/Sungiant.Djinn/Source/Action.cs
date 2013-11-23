using System;
using Sungiant.Cloud;
using Sungiant.Core;
using System.Collections.Generic;

namespace Sungiant.Djinn
{
	public abstract class Action
	{
		public string Description { get; private set; } 

		protected Action (String description)
		{
			Description = description;
		}

		protected void LogPerform()
		{
			Console.WriteLine(" --> Performing " + GetType() + " Action: " + Description);
		}

		public abstract void Perform(ICloudProvider cloud, ICloudDeployment deployment);
	}
}

