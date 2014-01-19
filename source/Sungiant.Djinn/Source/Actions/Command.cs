using System;
using Sungiant.Cloud;

using DjinnCommand = Sungiant.Djinn.Command;

namespace Sungiant.Djinn.Actions
{
	public class Command
		: Action<Specification.Command>
	{
		public Command (Specification.Command specification, String djinnContext) 
			: base (specification, djinnContext) {}
	
		MachineContext Context
		{ 
			get { return Specification.IsContextRemote ? MachineContext.Remote : MachineContext.Local; }
		}

		public override DjinnCommand[] GetRunnableCommands (ICloudProvider cloudProvider, ICloudDeployment cloudDeployment)
		{
			var result = new DjinnCommand[] {
				new DjinnCommand {
					MachineContext = Context,
					IgnoreErrors = Specification.IgnoreFailure,
					Value = Specification.Value
				}
			};

			return result;
		}
	}
}

