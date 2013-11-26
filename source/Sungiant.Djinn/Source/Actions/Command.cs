using System;
using Sungiant.Cloud;
using Sungiant.Core;

namespace Sungiant.Djinn
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

		public override void Perform (ICloudProvider cloudProvider, ICloudDeployment cloudDeployment)
		{
			LogPerform ();

			switch (Context)
			{
				case MachineContext.Local: 
					{
						// todo: this should always be run from the DjinnContext
						Int32 exitCode = ProcessHelper.Run (Specification.Value, Console.WriteLine);

						if (exitCode != 0 && !Specification.IgnoreFailure)
						{
							throw new Exception ("Exited with code " + exitCode);
						}
					}
					break;
				case MachineContext.Remote: 
					{
						cloudProvider.RunCommand (cloudDeployment, Specification.Value, Specification.IgnoreFailure); 
					}
					break;
			}
		}
	}
}

