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
			Arguments = string.Empty;
		}

		public ActionContext ActionContext { get; set; }

		public String Name { get; set; }
		public String Arguments { get; set; }

		public override void Perform(ICloudProvider cloudProvider, ICloudDeployment cloudDeployment, String localContext)
		{
			LogPerform();


			switch(ActionContext)
			{
				case ActionContext.Local: ProcessHelper.Run(Name, Arguments, Console.WriteLine); break;
				case ActionContext.Remote: cloudProvider.RunCommand(cloudDeployment, Name, Arguments); break;
			}
		}
	}
}

