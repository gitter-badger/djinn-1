using System;
using Sungiant.Cloud;
using System.IO;
using System.Collections.Generic;
using ServiceStack.Text;
using System.Linq;

using DjinnCommand = Sungiant.Djinn.Command;

namespace Sungiant.Djinn.Actions
{
	public class Scp
		: Action<Specification.Scp>
	{
		public Scp(Specification.Scp specification, String djinnContext) 
			: base(specification, djinnContext) {}

		String Source
		{
			get
			{
				switch (SourceContext)
				{ 
					case MachineContext.Local: return Path.Combine (DjinnContext, Specification.Source);
					case MachineContext.Remote: default: return Specification.Source;
				}
			}
		}

		MachineContext SourceContext
		{
			get { return Specification.IsSourceContextRemote ? MachineContext.Remote : MachineContext.Local; }
		}

		String Destination
		{
			get
			{
				switch (DestinationContext)
				{ 
					case MachineContext.Local: return Path.Combine (DjinnContext, Specification.Destination);
					case MachineContext.Remote: default: return Specification.Destination;
				}
			}
		}

		MachineContext DestinationContext
		{
			get { return Specification.IsDestinationContextRemote ? MachineContext.Remote : MachineContext.Local; }
		}

		String[] Arguments
		{
			get
			{
				var result = new List<String>();

				if (Specification.Recursive) result.Add("-r");
				if (Specification.Verbose) result.Add("-v");
				if (Specification.Quiet) result.Add("-q");

				return result.ToArray();
			}
		}

		public override DjinnCommand[] GetRunnableCommands (ICloudProvider cloudProvider, ICloudDeployment cloudDeployment)
		{
			var commands = new List<String> ();

			if (SourceContext == MachineContext.Local && DestinationContext == MachineContext.Local ||
				SourceContext == MachineContext.Remote && DestinationContext == MachineContext.Remote)
			{
				commands.Add (
					new String[] {
						"scp",
						Arguments.Join (" "),
						Source,
						Destination
					}.Join (" ")
				);
			}
			else if (SourceContext == MachineContext.Local && DestinationContext == MachineContext.Remote)
			{
				foreach (var endpoint in cloudDeployment.Endpoints)
				{
					commands.Add (
						new String[] {
							"scp",
							Arguments.Join(" "),
							"-o StrictHostKeyChecking=no",
							String.Format("-i {0}", cloudProvider.PrivateKeyPath),
							Source,
							String.Format("{0}@{1}:{2}", cloudProvider.User, endpoint, Destination)
						}.Join(" "));
				}
			}
			else if (SourceContext == MachineContext.Remote && DestinationContext == MachineContext.Local)
			{
				// todo, this is broken, because ypu might have 4 machines rsyncing down to just one
				foreach (var endpoint in cloudDeployment.Endpoints)
				{
					commands.Add (
						new String[]
						{
							"scp",
							Arguments.Join(" "),
							"-o StrictHostKeyChecking=no",
							String.Format("-i {0}", cloudProvider.PrivateKeyPath),
							String.Format("{0} {1}:{2}", cloudProvider.User, endpoint, Source),
							Path.Combine (Destination, endpoint)
						}.Join(" ")
					);
				}
			}

			var result = commands
				.Select (x => 
					new DjinnCommand () 
					{ 
						MachineContext = SourceContext,
						Value = x 
					})
				.ToArray ();

			return result;
		}
	}
}

