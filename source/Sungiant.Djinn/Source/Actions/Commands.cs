using System;
using Sungiant.Cloud;
using Sungiant.Core;

namespace Sungiant.Djinn
{
	public class Commands
		: Action<Specification.Commands>
	{
		public Commands (Specification.Commands specification, String djinnContext) 
			: base (specification, djinnContext) {}

		MachineContext Context
		{ 
			get { return Specification.IsContextRemote ? MachineContext.Remote : MachineContext.Local; }
		}

		public override void Perform (ICloudProvider cloudProvider, ICloudDeployment cloudDeployment)
		{
			LogPerform ();

			switch (Context)
			{
				case MachineContext.Local: 
					{
						// todo: this should always be run from the DjinnContext
						foreach (String val in Specification.Values)
						{
							Int32 exitCode = ProcessHelper.Run (val, Console.WriteLine);

							if (exitCode != 0 && !Specification.IgnoreFailure)
							{
								throw new Exception ("Exited with code " + exitCode);
							}
						}
					}
					break;
				case MachineContext.Remote: 
					{
						cloudProvider.RunCommands (cloudDeployment, Specification.Values, Specification.IgnoreFailure); 
					}
					break;
			}
		}
	}
}

