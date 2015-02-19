using System;
using System.Collections.Generic;
using Sungiant.Cloud;
using ServiceStack.Text;
using System.Linq;
using System.IO;

using DjinnCommand = Sungiant.Djinn.Command;

namespace Sungiant.Djinn.Actions
{
    public class UpstartDaemon
        : Action<Specification.UpstartDaemon>
    {
        public UpstartDaemon(Specification.UpstartDaemon specification, String djinnContext) 
            : base(specification, djinnContext) {}


        public override DjinnCommand[] GetRunnableCommands (ICloudProvider cloudProvider, ICloudDeployment cloudDeployment)
        {
            var result = new List<DjinnCommand> ();

            String filename = Specification.DaemonName + ".conf";

            String tempUpstartJobPath = Path.GetTempPath() + "upstart-" + filename;

            String[] upstartScript = new String[]
            {
                "start on started networking",
                "stop on shutdown",
                "respawn",
                "respawn limit 10 90",
                String.Format("env CLOUD_DEPLOYMENT_IDENTIFIER=\"{0}\"", cloudDeployment.Identifier.IdentiferTags.First().Value),
                String.Join("\n", Specification.DaemonCommands),
                "console output",
                ""
            };

            File.WriteAllText(tempUpstartJobPath, String.Join("\n", upstartScript));


            // Stop the service (ignoring errors as it might already exist)
            result.Add (
                new DjinnCommand {
                    MachineContext = MachineContext.Remote,
                    Value = "sudo service " + Specification.DaemonName + " stop",
                    IgnoreErrors = true
                });

            // Copy across the Nginx server configuration
            result.AddRange (
                new Rsync (
                    new Sungiant.Djinn.Specification.Rsync {
                        Source = tempUpstartJobPath,
                        Destination = filename
                    },
                    this.DjinnContext)
                .GetRunnableCommands (cloudProvider, cloudDeployment));

            result.Add (
                new DjinnCommand {
                    MachineContext = MachineContext.Remote,
                    Value = "sudo mv " + filename + " /etc/init/" + filename,
                    IgnoreErrors = true
                });

            result.Add (
                new DjinnCommand {
                    MachineContext = MachineContext.Remote,
                    Value = "sudo service " + Specification.DaemonName + " start",
                    IgnoreErrors = true
                });

            // Check that Nginx is ok 5 seconds later
            result.Add (
                new DjinnCommand {
                    MachineContext = MachineContext.Local,
                    Value = "sleep 5"
                });

            result.Add (
                new DjinnCommand {
                    MachineContext = MachineContext.Remote,
                    Value = "sudo service " + Specification.DaemonName + " status",
                    IgnoreErrors = true
                });

            return result.ToArray();
        }
    }
}
