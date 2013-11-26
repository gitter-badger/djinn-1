using System;
using System.Collections.Generic;
using Sungiant.Cloud;
using Sungiant.Core;
using ServiceStack.Text;
using System.Linq;
using System.IO;


namespace Sungiant.Djinn
{

	public class ReturnLocationBlock
		: NginxLocationBlock<Specification.ReturnLocationBlock>
	{
		public ReturnLocationBlock(Specification.ReturnLocationBlock specification)
			: base(specification) {}

		public override String[] GetConfig()
		{
			return new String[]
			{
				"location " + Specification.Location + " {",
				Specification.Return == null ? String.Empty : "  return  " + Specification.Return + ";",
				"}",
			};
		}
	}

	public class StaticLocationBlock
		: NginxLocationBlock<Specification.StaticLocationBlock>
	{
		public StaticLocationBlock(Specification.StaticLocationBlock specification)
			: base(specification) {}

		public override String[] GetConfig()
		{
			return new String[]
			{
				"location " + Specification.Location + " {",
				Specification.Root == null ? String.Empty : "  root  " + Specification.Root + ";",
				Specification.Rewrite == null ? String.Empty : "  rewrite " + Specification.Rewrite + ";",
				Specification.Index == null ? String.Empty : "  index " + Specification.Index + ";",
				Specification.TryFiles == null ? String.Empty : "  try_files " + Specification.TryFiles + ";",
				"}",
			};
		}
	}

	public class ProxyPassLocationBlock
		: NginxLocationBlock<Specification.ProxyPassLocationBlock>
	{
		public ProxyPassLocationBlock(Specification.ProxyPassLocationBlock specification)
			: base(specification) {}

		public override String[] GetConfig()
		{
			return new String[]
			{
				"location " + Specification.Location + " {",
				Specification.Rewrite == null ? String.Empty : "  rewrite " + Specification.Rewrite + ";",
				"  proxy_pass " + Specification.ProxyPass + ";",
				"  proxy_set_header  X-Remote-Address  $remote_addr;",
				"  proxy_set_header  X-Host  $host;",
				"}",
			};
		}
	}

	public abstract class NginxLocationBlock
	{
		public abstract String[] GetConfig();

		public static NginxLocationBlock CreateFromSpecification(Specification.INginxLocationBlock specification)
		{
			Type t = Type.GetType ("Sungiant.Djinn." + specification.Type + ", Sungiant.Djinn");

			Object o = Activator.CreateInstance (t, specification);

			return o as NginxLocationBlock;
		}
	}

	public abstract class NginxLocationBlock<T>
		: NginxLocationBlock
	where T
		: Specification.INginxLocationBlock
	{
		readonly T specification;

		protected T Specification { get { return specification; } }

		protected NginxLocationBlock (T specification)
		{
			this.specification = specification;
		}
	}

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

		// todo, ssl and certs
		public override void Perform(ICloudProvider cloudProvider, ICloudDeployment cloudDeployment)
		{
			LogPerform();
			
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
			
			cloudProvider.RunCommand(cloudDeployment, "sudo service " + "nginx stop");
			
			foreach( var endpoint in cloudDeployment.Endpoints )
			{
				ProcessHelper.Run(
					new String[]
					{
						"rsync",
						"-v",
						String.Format("--rsh \"ssh -o StrictHostKeyChecking=no -i {0}\"", cloudProvider.PrivateKeyPath),
						tempNginxConfigFile,
						String.Format("{0}@{1}:{2}", cloudProvider.User, endpoint, filename)
					}.Join(" "),
				Console.WriteLine);
			}
			
			cloudProvider.RunCommand(cloudDeployment, "sudo mv "+ filename + " /etc/nginx/sites-enabled/" + Specification.Name);
			cloudProvider.RunCommand(cloudDeployment, "sudo service "+ "nginx start");
			cloudProvider.RunCommand(cloudDeployment, "sudo service "+ "nginx status");

		}
	}
}

