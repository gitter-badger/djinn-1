using System;
using Sungiant.Cloud;
using Sungiant.Core;
using System.IO;
using System.Collections.Generic;
using ServiceStack.Text;

namespace Sungiant.Djinn
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

		public override void Perform(ICloudProvider cloudProvider, ICloudDeployment cloudDeployment)
		{
			LogPerform();

			if( SourceContext == MachineContext.Local && DestinationContext == MachineContext.Local )
			{
				ProcessHelper.Run(
					new String[]
					{
						"scp",
						Arguments.Join(" "),
						Source,
						Destination
					}.Join(" "),
					Console.WriteLine);

				return;
			}
			
			if( SourceContext == MachineContext.Remote && DestinationContext == MachineContext.Remote )
			{
				cloudProvider.RunCommand(
					cloudDeployment,
					new String[]
					{
						"scp",
						Arguments.Join(" "),
						Source,
						Destination
					}.Join(" "));

				return;
			}
			
			if( SourceContext == MachineContext.Local && DestinationContext == MachineContext.Remote )
			{
				foreach( var endpoint in cloudDeployment.Endpoints )
				{
					ProcessHelper.Run (
						new String[]
						{
							"scp",
							Arguments.Join(" "),
							"-o StrictHostKeyChecking=no",
							String.Format("-i {0}", cloudProvider.PrivateKeyPath),
							Source,
							String.Format("{0}@{1}:{2}", cloudProvider.User, endpoint, Destination)
						}.Join(" "),
						Console.WriteLine);
				}
				return;
			}
			
			if( SourceContext == MachineContext.Remote && DestinationContext == MachineContext.Local )
			{
				foreach( var endpoint in cloudDeployment.Endpoints )
				{
					ProcessHelper.Run (
						new String[]
						{
							"scp",
							Arguments.Join(" "),
							"-o StrictHostKeyChecking=no",
							String.Format("-i {0}", cloudProvider.PrivateKeyPath),
							String.Format("{0} {1}:{2}", cloudProvider.User, endpoint, Source),
							Destination
						}.Join(" "),
						Console.WriteLine);
				}
				return;
			}
		}
	}
}

