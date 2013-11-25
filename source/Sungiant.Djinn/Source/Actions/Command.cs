using System;
using Sungiant.Cloud;
using Sungiant.Core;

namespace Sungiant.Djinn
{
	public class Command
		: Action
	{
		public Command(String description) : base(description)
		{
			Value = string.Empty;
		}

		public ActionContext ActionContext { get; set; }

		public String Value { get; set; }

		public Boolean IgnoreFailure { get; set; }

		public override void Perform(ICloudProvider cloudProvider, ICloudDeployment cloudDeployment, String localContext)
		{
			LogPerform();

			switch(ActionContext)
			{
				case ActionContext.Local: ProcessHelper.Run(Value, "", Console.WriteLine); break;
				case ActionContext.Remote: cloudProvider.RunCommand(cloudDeployment, Value, IgnoreFailure); break;
			}
		}
	}
}

