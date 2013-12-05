using System;
using System.Collections.Generic;
using Sungiant.Cloud;
using Sungiant.Core;
using ServiceStack.Text;
using System.Linq;
using System.IO;

using DjinnCommand = Sungiant.Djinn.Command;


namespace Sungiant.Djinn.Actions
{
	public class SslConfig
	{
		public String Certificate { get; set; }
		public String CertificateKey { get; set; }
	}

	public class NginxServerBlock
		: Action<Specification.NginxServerBlock>
	{
		public NginxServerBlock(Specification.NginxServerBlock specification, String djinnContext) 
			: base(specification, djinnContext) {}

		public override DjinnCommand[] GetRunnableCommands (ICloudProvider cloudProvider, ICloudDeployment cloudDeployment)
		{
			string filename = Specification.Name + ".conf";
			
			string tempNginxConfigFile = Path.GetTempPath() + "nginx-" + filename;

			var nginxScript = new List<string>();

			nginxScript.Add( "server {" );

			if (string.IsNullOrEmpty(Specification.Listen))
			{
				nginxScript.Add ("  listen 80;");
			}
			else
			{
				nginxScript.Add ("  listen " + Specification.Listen + ";");
			}

			if (Specification.VirtualHosts != null)
				nginxScript.Add( "  server_name " + Specification.VirtualHosts.Join(" ") + ";" );

			if (Specification.SslConfig != null)
			{
				nginxScript.Add ("  ssl on;");
				nginxScript.Add (string.Format("  ssl_certificate {0};", Specification.SslConfig.Certificate));
				nginxScript.Add (string.Format("  ssl_certificate_key {0};", Specification.SslConfig.CertificateKey));
			}

			if( Specification.AllowedEndpoints != null )
			{
				foreach(var endpoint in Specification.AllowedEndpoints)
				{
					nginxScript.Add( "  allow " + endpoint +";" );
				}
				
				nginxScript.Add( "  deny all;" );
			}

			if (!string.IsNullOrEmpty (Specification.Return))
			{
				nginxScript.Add( "  return " + Specification.Return + ";" );
			}

			if (Specification.Locations != null)
			{
				var locations = Specification.Locations
					.Select (x => NginxLocationBlock.CreateFromSpecification (x))
					.ToList ();

				foreach (var location in locations)
				{
					var locBlock = location
						.GetConfig ()
						.Where (x => !String.IsNullOrEmpty (x));

					foreach (var line in locBlock)
					{
						nginxScript.Add ("  " + line);
					}
				}
			}

			nginxScript.Add( "}" );
			nginxScript.Add( "" );

			File.WriteAllText(tempNginxConfigFile, String.Join("\n", nginxScript));

			var result = new List<DjinnCommand> ();


			// Stop Nginx
			result.Add (
				new DjinnCommand {
					MachineContext = MachineContext.Remote,
					Value = "sudo service " + "nginx stop"
				});

			// Copy across the Nginx server configuration
			result.AddRange (
				new Rsync (
					new Sungiant.Djinn.Specification.Rsync {
						Source = tempNginxConfigFile,
						Destination = filename
					},
					this.DjinnContext)
				.GetRunnableCommands (cloudProvider, cloudDeployment));

			// Move the file into the right place
			result.Add (
				new DjinnCommand {
					MachineContext = MachineContext.Remote,
					Value = "sudo mv "+ filename + " /etc/nginx/sites-enabled/" + Specification.Name
				});

			// Start Nginx
			result.Add (
				new DjinnCommand {
					MachineContext = MachineContext.Remote,
					Value = "sudo service "+ "nginx start"
				});

			// Check that Nginx is ok 5 seconds later
			result.Add (
				new DjinnCommand {
					MachineContext = MachineContext.Local,
					Value = "sleep 5"
				});

			// Show the Nginx status
			result.Add (
				new DjinnCommand {
					MachineContext = MachineContext.Remote,
					Value = "sudo service "+ "nginx status"
				});

			return result.ToArray();
		}
	}
}

