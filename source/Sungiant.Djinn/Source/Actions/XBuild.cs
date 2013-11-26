using System;
using Sungiant.Cloud;
using Sungiant.Core;
using ServiceStack.Text;
using System.IO;

namespace Sungiant.Djinn
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

		public override void Perform (ICloudProvider cloudProvider, ICloudDeployment cloudDeployment)
		{
			LogPerform();

			if (string.IsNullOrEmpty (Specification.Verbosity))
			{
				Specification.Verbosity = "minimal";
			}

			var command = new string[]
			{
				"xbuild",
				"\"" + ProjectPath + "\"",
				string.Format("/p:Configuration={0}", Specification.Configuration),
				string.Format("/verbosity:{0}", Specification.Verbosity)
			}.Join(" ");

			switch(Context)
			{
				case MachineContext.Local: ProcessHelper.Run(command, Console.WriteLine); break;
				case MachineContext.Remote: cloudProvider.RunCommand(cloudDeployment, command); break;
			}
		}
	}
}

