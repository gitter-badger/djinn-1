using System;
using System.Collections.Generic;
using Sungiant.Cloud;
using Sungiant.Core;
using ServiceStack.Text;
using System.Linq;
using System.IO;


namespace Sungiant.Djinn
{
	public class StaticLocationBlock
		: NginxLocationBlock
	{
		public String Root { get; set; }

		public String Index { get; set; }

		public String TryFiles { get; set; }

		public override String[] GetConfig()
		{
			return new String[]
			{
				"location " + Location + " {",
				Root == null ? string.Empty : "  root  " + Root + ";",
				Rewrite == null ? string.Empty : "  rewrite " + Rewrite + ";",
				Index == null ? string.Empty : "  index " + Index + ";",
				TryFiles == null ? string.Empty : "  try_files " + TryFiles + ";",
				"}",
			};
		}
	}

	public class ProxyPassLocationBlock
		: NginxLocationBlock
	{
		public String ProxyPass { get; set; }


		public override String[] GetConfig()
		{
			return new String[]
			{
				"location " + Location + " {",
				Rewrite == null ? string.Empty : "  rewrite " + Rewrite + ";",
				"  proxy_pass " + ProxyPass + ";",
				"  proxy_set_header  X-Remote-Address  $remote_addr;",
				"  proxy_set_header  X-Host  $host;",
				"}",
			};
		}
	}

	public abstract class NginxLocationBlock
	{
		public String Location { get; set; }
		
		public String Rewrite { get; set; }

		public abstract String[] GetConfig();
	}


	public class NginxServerBlock
		: Action
	{
		public NginxServerBlock(String description) 
			: base(ActionType.NginxServerBlock, description)
		{
		}

		public String Name { get; set; }
		
		public List<String> VirtualHosts { get; set; }

		public List<String> AllowedEndpoints { get; set; }

		public List<NginxLocationBlock> Locations { get; set; }

		// todo, ssl and certs
		public override void Perform(ICloudProvider cloudProvider, ICloudDeployment cloudDeployment)
		{
			LogPerform();
			
			string filename = Name + ".conf";
			
			string tempNginxConfigFile = Path.GetTempPath() + "nginx-" + filename;

			var nginxScript = new List<string>();

			nginxScript.Add( "server {" );
			nginxScript.Add( "  listen 80;" );
			nginxScript.Add( "  server_name " + VirtualHosts.Join(" ") + ";" );

			if( AllowedEndpoints != null )
			{
				foreach(var endpoint in AllowedEndpoints)
				{
					nginxScript.Add( "  allow " + endpoint +";" );
				}
				
				nginxScript.Add( "  deny all;" );
			}

			foreach( var location in Locations )
			{
				var locBlock = location
					.GetConfig()
					.Where(x => !string.IsNullOrEmpty(x));

				foreach( var line in locBlock )
				{
					nginxScript.Add( "  " + line );
				}
			}

			nginxScript.Add( "}" );
			nginxScript.Add( "" );
			
			File.WriteAllText(tempNginxConfigFile, string.Join("\n", nginxScript));
			
			cloudProvider.RunCommand(cloudDeployment, "sudo service", "nginx stop");
			
			foreach( var endpoint in cloudDeployment.Endpoints )
			{
				ProcessHelper.Run(
					"rsync",
					new string[]
					{
						"-v",
						string.Format("--rsh \"ssh -o StrictHostKeyChecking=no -i {0}\"", cloudProvider.PrivateKeyPath),
						tempNginxConfigFile,
						string.Format("{0}@{1}:{2}", cloudProvider.User, endpoint, filename)
					}.Join(" "),
				Console.WriteLine
				);
			}
			
			cloudProvider.RunCommand(cloudDeployment, "sudo mv", filename + " /etc/nginx/sites-enabled/" + Name);
			cloudProvider.RunCommand(cloudDeployment, "sudo service", "nginx start");
			cloudProvider.RunCommand(cloudDeployment, "sudo service", "nginx status");

		}
	}
}

