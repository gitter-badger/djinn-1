using System;
using System.Collections.Generic;
using Sungiant.Cloud;
using Sungiant.Core;
using ServiceStack.Text;
using System.Linq;
using System.IO;

namespace Sungiant.Djinn
{
	public class UpstartDaemon
		: Action
	{
		public UpstartDaemon(String description) 
			: base(ActionType.UpstartDaemon, description)
		{
		}
		
		public String DaemonName { get; set; }
		public List<String> DaemonCommands { get; set; }

		public override void Perform(ICloudProvider cloudProvider, ICloudDeployment cloudDeployment)
		{
			LogPerform();

			string filename = DaemonName + ".conf";

			string tempUpstartJobPath = Path.GetTempPath() + "upstart-" + filename;

			string[] upstartScript = new string[]
			{
				"start on started networking",
				"stop on shutdown",
				"respawn",
				"respawn limit 10 90",
				string.Format("env CLOUD_DEPLOYMENT_IDENTIFIER=\"{0}\"", cloudDeployment.Identifier.IdentiferTags.First().Value),
				string.Join("\n", DaemonCommands),
				"console output",
				""
			};

			File.WriteAllText(tempUpstartJobPath, string.Join("\n", upstartScript));
			
			cloudProvider.RunCommand(cloudDeployment, "sudo service", DaemonName + " stop");

			foreach( var endpoint in cloudDeployment.Endpoints )
			{
				ProcessHelper.Run(
					"rsync",
					new string[]
					{
						"-v",
						string.Format("--rsh \"ssh -o StrictHostKeyChecking=no -i {0}\"", cloudProvider.PrivateKeyPath),
						tempUpstartJobPath,
						string.Format("{0}@{1}:{2}", cloudProvider.User, endpoint, filename)
					}.Join(" "),
				Console.WriteLine
				);
			}

			cloudProvider.RunCommand(cloudDeployment, "sudo mv", filename + " /etc/init/" + filename);
			cloudProvider.RunCommand(cloudDeployment, "sudo service", DaemonName + " start");
			cloudProvider.RunCommand(cloudDeployment, "sudo service", DaemonName + " status");

		}
	}
}
