using System;
using Sungiant.Cloud;
using ServiceStack.Text;
using System.IO;

using DjinnCommand = Sungiant.Djinn.Command;

namespace Sungiant.Djinn.Actions
{
	public class XBuild
		: Action<Specification.XBuild>
	{
		public XBuild (Specification.XBuild specification, String djinnContext) 
			: base (specification, djinnContext) {}

		String ProjectPath
		{
			get
			{
				switch(Context)
				{
					case MachineContext.Local: return Path.Combine (DjinnContext, Specification.ProjectPath);
					case MachineContext.Remote: default: return Specification.ProjectPath;
				}
			}
		}

		MachineContext Context
		{ 
			get { return Specification.IsContextRemote ? MachineContext.Remote : MachineContext.Local; }
		}

		public override DjinnCommand[] GetRunnableCommands (ICloudProvider cloudProvider, ICloudDeployment cloudDeployment)
		{
			if (String.IsNullOrEmpty (Specification.Verbosity))
			{
				Specification.Verbosity = "minimal";
			}

			var command = new String[]
			{
				"xbuild",
				"\"" + ProjectPath + "\"",
				String.Format("/p:Configuration={0}", Specification.Configuration),
				String.Format("/verbosity:{0}", Specification.Verbosity)
			}.Join(" ");

			var result = new DjinnCommand[] {
				new DjinnCommand {
					MachineContext = Context,
					Value = command
				}
			};

			return result;
		}
	}
}

