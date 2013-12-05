using System;
using Sungiant.Cloud;

namespace Sungiant.Djinn
{
	public abstract class Task
	{
		protected Task(TaskType task, ICloudProvider cloudProvider, Deployment deployment)
		{
			TaskType = task;
			CloudProvider = cloudProvider;
			Deployment = deployment;
		}

		TaskType TaskType { get; set; }

		protected ICloudProvider CloudProvider { get; private set; }
		protected Deployment Deployment { get; private set; }

		public abstract void Run();
	}
}

