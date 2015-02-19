using System;
using Sungiant.Cloud;
using ServiceStack.Text;

using DjinnCommand = Sungiant.Djinn.Command;

namespace Sungiant.Djinn.Actions
{
    public class Aptitude
        : Action<Specification.Aptitude>
    {
        public Aptitude (Specification.Aptitude specification, String djinnContext) 
            : base (specification, djinnContext) {}

        public override DjinnCommand[] GetRunnableCommands (ICloudProvider cloudProvider, ICloudDeployment cloudDeployment)
        {
            var result = new DjinnCommand[] {
                new DjinnCommand {
                    MachineContext = MachineContext.Remote,//todo, don't hard code, have this in base type
                    Value = "sudo apt-get -q -y update"
                },
                new DjinnCommand {
                    MachineContext = MachineContext.Remote,//todo, don't hard code, have this in base type
                    Value = "sudo apt-get -q -y install " + this.Specification.Install.Join (" ")
                }
            };

            return result;
        }
    }
}
