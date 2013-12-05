using System;
using Sungiant.Cloud;
using System.Collections.Generic;
using Sungiant.Core;

namespace Sungiant.Djinn
{
	public class CommandRunner
	{
		readonly ICloudProvider cloudProvider;
		readonly ICloudDeployment cloudDeployment;

		public CommandRunner(
			ICloudProvider cloudProvider, 
			ICloudDeployment cloudDeployment)
		{
			this.cloudProvider = cloudProvider;
			this.cloudDeployment = cloudDeployment;
		}

		List<Command> AggregateCommands(ActionGroup actionGroup)
		{
			var allCommands = new List<Command> ();

			foreach( var action in actionGroup.Actions )
			{
				var c = action.GetRunnableCommands (this.cloudProvider, this.cloudDeployment);

				action.LogDetails ();
				allCommands.AddRange (c);
			}

			return allCommands;
		}

		public void RunBatch(ActionGroup actionGroup)
		{
			var commands = AggregateCommands(actionGroup);

			// grab the actions steps and group them together
			var currentCommands = new List<String>();
			Boolean? currentIgnoreErrors = null;
			MachineContext? currentMachineContext = null;

			foreach (var step in commands)
			{
				Boolean noCurrentCommands = (currentCommands.Count == 0);
				Boolean matchingCommandType = 
					(currentMachineContext == step.MachineContext && currentIgnoreErrors == step.IgnoreErrors);

				if (noCurrentCommands || matchingCommandType)
				{
					currentIgnoreErrors = step.IgnoreErrors;
					currentMachineContext = step.MachineContext;
					currentCommands.Add (step.Value);
					continue;
				}

				switch (currentMachineContext.Value)
				{
					case MachineContext.Local: 
						{
							// todo: this should always be run from the DjinnContext
							foreach (String val in currentCommands)
							{
								Int32 exitCode = ProcessHelper.Run (val, Console.WriteLine);

								if (exitCode != 0 && !currentIgnoreErrors.Value)
								{
									throw new Exception ("Exited with code " + exitCode);
								}
							}
						}
						break;
					case MachineContext.Remote: 
						{
							cloudProvider.RunCommands (cloudDeployment, currentCommands.ToArray(), currentIgnoreErrors.Value); 
						}
						break;
				}

				//run grouped command
				currentCommands.Clear ();
				currentIgnoreErrors = null;
				currentMachineContext = null;
			}
		}

	}
}

