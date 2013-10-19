using System;
using Sungiant.Cloud;

namespace Sungiant.Djinn
{
	public abstract class DjinnTask
	{
		protected DjinnTask(Task task, ICloudProvider cloudProvider, Deployment deployment)
		{
			Task = task;
			CloudProvider = cloudProvider;
			Deployment = deployment;
		}

		Task Task { get; set; }

		protected ICloudProvider CloudProvider { get; private set; }
		protected Deployment Deployment { get; private set; }

		public abstract void Run();
	}
}

