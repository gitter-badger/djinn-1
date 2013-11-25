using System;
using Sungiant.Cloud;
using Sungiant.Core;
using ServiceStack.Text;
using System.IO;

namespace Sungiant.Djinn
{
	public class XBuild
		: Action
	{
		public XBuild(String description) 
			: base(description) { }

		public String Configuration { get; set; }

		public String ProjectPath { get; set; }

		public String Verbosity  { get; set; }

		// is this run here or on the remote machine
		public ActionContext Context { get; set; }

		public override void Perform(ICloudProvider cloudProvider, ICloudDeployment cloudDeployment, String localContext)
		{
			LogPerform();

			var command = "xbuild";

			String blueprintsDirectory = Path.Combine (localContext, "blueprints");
			var projFullPath = Path.Combine (blueprintsDirectory, ProjectPath);

			if (string.IsNullOrEmpty (Verbosity))
			{
				Verbosity = "minimal";
			}

			var arguments = new string[]
			{
				"\"" + projFullPath + "\"",
				string.Format("/p:Configuration={0}", Configuration),
				string.Format("/verbosity:{0}", Verbosity)
			}.Join(" ");

			String cmd = command + " " + arguments;

			switch(Context)
			{
				case ActionContext.Local: ProcessHelper.Run(command, arguments, Console.WriteLine); break;
				case ActionContext.Remote: cloudProvider.RunCommand(cloudDeployment, cmd); break;
			}
		}
	}
}

