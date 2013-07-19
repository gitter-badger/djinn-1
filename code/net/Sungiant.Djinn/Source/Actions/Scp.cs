using System;
using Sungiant.Cloud;
using Sungiant.Core;
using System.IO;
using System.Collections.Generic;
using ServiceStack.Text;

namespace Sungiant.Djinn
{
	public class Scp
		: Action
	{
		public Scp(String description) : base(ActionType.Scp, description) { }

		// sets the -r flag
		public Boolean Recursive { get; set; }
		
		// sets the -v flag
		public Boolean Verbose { get; set; }
		
		// sets the -q flag
		public Boolean Quiet { get; set; }

		public String SourcePath { get; set; }
		public ActionContext SourceContext { get; set; }
		
		public String DestinationPath { get; set; }
		public ActionContext DestinationContext { get; set; }

		string[] Arguments
		{
			get
			{
				var result = new List<string>();

				if( Recursive ) result.Add("-r");
				if( Verbose ) result.Add("-v");
				if( Quiet ) result.Add("-q");
				
				return result.ToArray();
			}
		}

		public override void Perform(ICloudProvider cloudProvider, ICloudDeployment cloudDeployment)
		{
			LogPerform();
			
			if( SourceContext == ActionContext.Local && DestinationContext == ActionContext.Local )
			{
				ProcessHelper.Run(
					"scp",
					new string[]
					{
						Arguments.Join(" "),
						SourcePath,
						DestinationPath
					}.Join(" "),
					Console.WriteLine
				);
				return;
			}
			
			if( SourceContext == ActionContext.Remote && DestinationContext == ActionContext.Remote )
			{
				cloudProvider.RunCommand(
					cloudDeployment,
					"scp",
					new string[]
					{
						Arguments.Join(" "),
						SourcePath,
						DestinationPath
					}.Join(" ")
				);
				return;
			}
			
			if( SourceContext == ActionContext.Local && DestinationContext == ActionContext.Remote )
			{
				
				
				foreach( var endpoint in cloudDeployment.Endpoints )
				{
					ProcessHelper.Run(
						"scp",
						new string[]
						{
							Arguments.Join(" "),
							"-o StrictHostKeyChecking=no",
							string.Format("-i {0}", cloudProvider.PrivateKeyPath),
							SourcePath,
							string.Format("{0}@{1}:{2}", cloudProvider.User, endpoint, DestinationPath)
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
						"scp",
						new string[]
						{
							Arguments.Join(" "),
							"-o StrictHostKeyChecking=no",
							string.Format("-i {0}", cloudProvider.PrivateKeyPath),
							string.Format("{0} {1}:{2}", cloudProvider.User, endpoint, SourcePath),
							DestinationPath
						}.Join(" "),
						Console.WriteLine
					);
				}
				return;
			}
		}
	}
}

