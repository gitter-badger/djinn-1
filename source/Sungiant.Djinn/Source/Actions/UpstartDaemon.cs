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
		: Action<Specification.UpstartDaemon>
	{
		public UpstartDaemon(Specification.UpstartDaemon specification, String djinnContext) 
			: base(specification, djinnContext) {}


		public override void Perform(ICloudProvider cloudProvider, ICloudDeployment cloudDeployment)
		{
			LogPerform();

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
			
			cloudProvider.RunCommand(cloudDeployment, "sudo service " + Specification.DaemonName + " stop", true);

			foreach( var endpoint in cloudDeployment.Endpoints )
			{
				ProcessHelper.Run(
					new String[]
					{
						"rsync",
						"-v",
						String.Format("--rsh \"ssh -o StrictHostKeyChecking=no -i {0}\"", cloudProvider.PrivateKeyPath),
						tempUpstartJobPath,
						String.Format("{0}@{1}:{2}", cloudProvider.User, endpoint, filename)
					}.Join(" "),
				Console.WriteLine);
			}

			cloudProvider.RunCommand(cloudDeployment, "sudo mv " + filename + " /etc/init/" + filename);
			cloudProvider.RunCommand(cloudDeployment, "sudo service " + Specification.DaemonName + " start");
			cloudProvider.RunCommand(cloudDeployment, "sudo service " + Specification.DaemonName + " status");

		}
	}
}
