using System;
using System.IO;
using Sungiant.Cloud;
using Sungiant.Core;
using ServiceStack.Text;

namespace Sungiant.Djinn
{
	public class MdToolBuild
		: Action
	{
		public MdToolBuild(String description) : base(description) { }

		public String Configuration { get; set; }
		
		public String SolutionPath { get; set; }

		public Boolean Verbose { get; set; } 
		// is this run here or on the remote machine
		public ActionContext Context { get; set; }

		public override void Perform(ICloudProvider cloudProvider, ICloudDeployment cloudDeployment, String localContext)
		{
			LogPerform();

			if(!File.Exists(SolutionPath))
			{
				throw new Exception("Solution file not found.");
			}

			string command = "/Applications/Xamarin Studio.app/Contents/MacOS/mdtool";

			var arguments = new string[]
			{
				"build",
				"\"--counfiguration:" + Configuration + "\"",
				string.Format("\"{0}\"", SolutionPath)
			}.Join(" ");

			if( Verbose )
			{
				arguments = "-v " + arguments;
			}

			String cmd = command + " " + arguments;

			switch(Context)
			{
				case ActionContext.Local: ProcessHelper.Run (command, arguments, Console.WriteLine); break;
				case ActionContext.Remote: cloudProvider.RunCommand(cloudDeployment, cmd); break;
			}

		}

	}
}

