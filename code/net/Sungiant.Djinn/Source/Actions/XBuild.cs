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

		public override void Perform(ICloudProvider cloudProvider, ICloudDeployment cloudDeployment)
		{
			LogPerform();

			var command = "xbuild";

			var projFullPath = DjinnConfiguration.Instance.ActiveWorkgroup.RepoDirectory + ProjectPath;

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

			switch(Context)
			{
				case ActionContext.Local: ProcessHelper.Run(command, arguments, Console.WriteLine); break;
				case ActionContext.Remote: cloudProvider.RunCommand(cloudDeployment, command, arguments); break;
			}
		}
	}
}

