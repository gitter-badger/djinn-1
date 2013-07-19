using System;
using Sungiant.Cloud;
using Sungiant.Core;
using System.IO;
using System.Collections.Generic;
using ServiceStack.Text;

namespace Sungiant.Djinn
{
	public class Rsync
		: Action
	{
		public Rsync(String description)
			: base(ActionType.Rsync, description)
		{
			Delete = true;
			Verbose = true;
			Recursive = true;
		}

		// sets the --delete flag
		public Boolean Delete { get; set; }

		// sets the -r flag
		public Boolean Recursive { get; set; }
		
		// sets the -v flag
		public Boolean Verbose { get; set; }
		
		// sets the -q flag
		public Boolean Quiet { get; set; }

		// sets the -c flag
		public Boolean Checksum { get; set; }

		// sets the -l flag
		public Boolean Shortcuts { get; set; }

		// sets the --stats flag
		public Boolean Stats { get; set; }

		public String Source { get; set; }


		public String ActualSource
		{ 
			get
			{
				if( SourceContext == ActionContext.Remote )
				{
					return Source;
				}
				else if( SourceContext == ActionContext.Local )
				{
					return DjinnConfiguration.Instance.ActiveWorkgroup.SpecFileDirectory + Source;
				}
				else throw new NotSupportedException();
			}
		}

		public ActionContext SourceContext { get; set; }

		public String Destination { get; set; }

		public String ActualDestination
		{ 
			get
			{
				if( DestinationContext == ActionContext.Remote )
					return Destination;
				else if( DestinationContext == ActionContext.Local )
				{
					return DjinnConfiguration.Instance.ActiveWorkgroup.SpecFileDirectory + Destination;
				}
				else throw new NotSupportedException();
			}
		}

		public ActionContext DestinationContext { get; set; }

		string[] Arguments
		{
			get
			{
				var result = new List<string>();
				
				if( Delete ) result.Add("--delete");
				if( Recursive ) result.Add("-r");
				if( Verbose ) result.Add("-v");
				if( Quiet ) result.Add("-q");
				if( Checksum ) result.Add("-c");
				if( Shortcuts ) result.Add("-l");
				if( Stats ) result.Add("--stats");

				return result.ToArray();
			}
		}

		public override void Perform(ICloudProvider cloudProvider, ICloudDeployment cloudDeployment)
		{
			LogPerform();

			if( SourceContext == ActionContext.Local && DestinationContext == ActionContext.Local )
			{
				ProcessHelper.Run(
					"rsync",
					Arguments.Join(" ") + " " + ActualSource + " " + ActualDestination, 
					Console.WriteLine
					);
				return;
			}
			
			if( SourceContext == ActionContext.Remote && DestinationContext == ActionContext.Remote )
			{
				cloudProvider.RunCommand(
					cloudDeployment,
					"rsync",
					new string[]
					{
						Arguments.Join(" "),
						ActualSource,
						ActualDestination
					}.Join(" ")
				);

				return;
			}
			
			if( SourceContext == ActionContext.Local && DestinationContext == ActionContext.Remote )
			{
				foreach( var endpoint in cloudDeployment.Endpoints )
				{
					ProcessHelper.Run(
						"rsync",
						new string[]
						{
							Arguments.Join(" "),
							string.Format("--rsh \"ssh -o StrictHostKeyChecking=no -i {0}\"", cloudProvider.PrivateKeyPath),
							ActualSource,
							string.Format("{0}@{1}:{2}", cloudProvider.User, endpoint, ActualDestination)
						}.Join(" "), 
					Console.WriteLine
					);
				}
				return;
			}
			
			if( SourceContext == ActionContext.Remote && DestinationContext == ActionContext.Local )
			{
				foreach( var endpoint in cloudDeployment.Endpoints )
				{
					ProcessHelper.Run(
						"rsync",
						new string[]
						{
							Arguments.Join(" "),
							string.Format("--rsh \"ssh -o StrictHostKeyChecking=no -i {0}\"", cloudProvider.PrivateKeyPath),
							string.Format("{0}@{1}:{2}", cloudProvider.User, endpoint, ActualSource),
							ActualDestination
						}.Join(" "),
						Console.WriteLine
					);
				}
				return;
			}
		}
	}
}

