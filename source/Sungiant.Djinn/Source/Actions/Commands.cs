using System;
using Sungiant.Cloud;
using System.Collections.Generic;

using DjinnCommand = Sungiant.Djinn.Command;

namespace Sungiant.Djinn.Actions
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

        public override DjinnCommand[] GetRunnableCommands (ICloudProvider cloudProvider, ICloudDeployment cloudDeployment)
        {
            var result = new List<DjinnCommand>();

            foreach (String value in Specification.Values)
            {
                var rcmd = new DjinnCommand {
                    MachineContext = Context,
                    IgnoreErrors = Specification.IgnoreFailure,
                    Value = value
                };

                result.Add (rcmd);
            }

            return result.ToArray();
        }
    }
}

