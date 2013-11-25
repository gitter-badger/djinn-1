// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ Djinn - IaaS Deployment Utility                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Brought to you by:                                                     │ \\
// │          _________                    .__               __             │ \\
// │         /   _____/__ __  ____    ____ |__|____    _____/  |_           │ \\
// │         \_____  \|  |  \/    \  / ___\|  \__  \  /    \   __\          │ \\
// │         /        \  |  /   |  \/ /_/  >  |/ __ \|   |  \  |            │ \\
// │        /_______  /____/|___|  /\___  /|__(____  /___|  /__|            │ \\
// │                \/           \//_____/         \/     \/                │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2013 A.J.Pook (http://sungiant.github.com)                 │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Permission is hereby granted, free of charge, to any person obtaining  │ \\
// │ a copy of this software and associated documentation files (the        │ \\
// │ "Software"), to deal in the Software without restriction, including    │ \\
// │ without limitation the rights to use, copy, modify, merge, publish,    │ \\
// │ distribute, sublicense, and/or sellcopies of the Software, and to      │ \\
// │ permit persons to whom the Software is furnished to do so, subject to  │ \\
// │ the following conditions:                                              │ \\
// │                                                                        │ \\
// │ The above copyright notice and this permission notice shall be         │ \\
// │ included in all copies or substantial portions of the Software.        │ \\
// │                                                                        │ \\
// │ THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,        │ \\
// │ EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF     │ \\
// │ MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. │ \\
// │ IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY   │ \\
// │ CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,   │ \\
// │ TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE       │ \\
// │ SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                 │ \\
// └────────────────────────────────────────────────────────────────────────┘ \\

using System;
using NDesk.Options;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ServiceStack.Text;
using Sungiant.Core;
using System.Text;
using Sungiant.Cloud;
using System.Threading;

namespace Sungiant.Djinn
{
	static class Djinn
    {
		public static DjinnEnvironment DjinnEnvironment { get; private set; }

		static Int32 LogLevel { get; set; }
		static Boolean ShowHelp { get; set; }

		static ICloudProvider CloudProvider;

		static OptionSet OptionSet;

		const string Version = "0.2.1";

		static Djinn()
		{
			LogLevel = 1;

			OptionSet = new OptionSet()
			{
				{ 
					"q|quiet=", 
					"reduce output level",
					v => { LogLevel--; } 
				},
				{ 
					"v|verbose=", 
					"increase output level",
					v =>  { LogLevel++; } 
				},
				{ 
					"h|?|help", 
					"shows the help message and exits", 
					v => ShowHelp = v != null 
				},
			};
		}

        public static void Main(string[] args)
        {
			
			Console.WriteLine ("________        ____.___ _______    _______   ");
			Console.WriteLine ("\\______ \\      |    |   |\\      \\   \\      \\");
			Console.WriteLine (" |    |  \\     |    |   |/   |   \\  /   |   \\ ");
			Console.WriteLine (" |    `   \\/\\__|    |   /    |    \\/    |    \\");
			Console.WriteLine ("/_______  /\\________|___\\____|__  /\\____|__  /");
			Console.WriteLine ("        \\/                      \\/         \\/ ");
			Console.WriteLine ("Djinn v" + Version);

			// loads up djinn's configuration file
			DjinnConfiguration.Instance.Load ();

			InitiliseCloudProvider();

			Console.WriteLine ("Active Workgroup: " + DjinnConfiguration.Instance.ActiveWorkgroup.Name);


			var environmentSpecification = new DjinnEnvironmentSetupData ();

			foreach (var projectConfig in DjinnConfiguration.Instance.ActiveWorkgroup.ProjectConfigurations)
			{
				var blueprint_specs = new List<BlueprintSpecification> ();
				var deployment_specs = new List<DeploymentSpecification> ();
				var zone_specs = new List<ZoneSpecification> ();

				foreach(var file in Directory.GetFiles(projectConfig.BlueprintsDirectory))
				{
					String allText = file.ReadAllText ();
					var spec = allText.FromXml<BlueprintSpecification> ();
					blueprint_specs.Add (spec);
				}

				foreach(var file in Directory.GetFiles(projectConfig.DeploymentsDirectory))
				{
					String allText = file.ReadAllText ();
					var spec = allText.FromXml<DeploymentSpecification> ();
					deployment_specs.Add (spec);
				}

				foreach(var file in Directory.GetFiles(projectConfig.ZonesDirectory))
				{
					String allText = file.ReadAllText ();
					var spec = allText.FromXml<ZoneSpecification> ();
					zone_specs.Add (spec);
				}


				environmentSpecification.AddProject (
					projectConfig.DjinnDirectory,
					blueprint_specs,
					deployment_specs,
					zone_specs
				);
			}

			DjinnEnvironment = new DjinnEnvironment(environmentSpecification);

			DjinnTask djinnTask = ParseArguments(args);

			if (ShowHelp || djinnTask == null) 
			{
				PrintHelp ();
				return;
			}

			djinnTask.Run();
			Console.WriteLine ("Completed " + djinnTask.GetType().ToString ());
        }

		static void InitiliseCloudProvider()
		{
			if (DjinnConfiguration.Instance.DjinnAwsFile != null && DjinnConfiguration.Instance.DjinnAzureFile != null)
			{
				throw new NotImplementedException("todo: do you want to use azure or aws?");
			}
			
			if (DjinnConfiguration.Instance.DjinnAwsFile != null)
			{
				CloudProvider = new Sungiant.Cloud.Aws.Aws(DjinnConfiguration.Instance.DjinnAwsFile);
			}
			
			if (DjinnConfiguration.Instance.DjinnAzureFile != null)
			{
				CloudProvider = new Sungiant.Cloud.Azure.Azure(DjinnConfiguration.Instance.DjinnAzureFile);
			}

		}

		static DjinnTask CreateFrom(String task, String extra, Deployment deployment)
		{
			if (task == "ssh")
			{
				return new DjinnSshTask(CloudProvider, deployment);
			}
			else if (task == "provision")
			{
				return new DjinnProvisionTask(CloudProvider, deployment);
			}
			else if (task == "deploy")
			{
				var deploy = new DjinnDeployTask(CloudProvider, deployment);
				deploy.SpecificActionGroup = extra;
				return deploy;
			}
			else if (task == "destroy")
			{
				return new DjinnDestroyTask(CloudProvider, deployment);
			}
			else if (task == "describe")
			{
				return new DjinnDescribeTask(CloudProvider, deployment);
			}
			else if (task == "configure")
			{
				var configure = new DjinnConfigureTask(CloudProvider, deployment);
				configure.SpecificActionGroup = extra;
				return configure;
			}

			return null;
		}

		static DjinnTask ParseArguments(String[] args)
		{
			if (args.Length < 3 )
			{
				ShowHelp = true;
				return null;
			}
			
			string task = args[0];
			string deploymentGroupId = args[1];
			string machineBlueprintId = args[2];
			string extra = null;

			if( args.Length == 4 )
			{
				extra = args[3];
			}

			OptionSet.Parse (args);

			// make sure the deployment we want to talk to exists
			var deployment = DjinnEnvironment.Deployments
				.Find (x => (x.DeploymentGroup.Id == deploymentGroupId && x.Blueprint.Id == machineBlueprintId));

			if (deployment == null)
			{
				ShowHelp = true;

				return null;
			}

			return CreateFrom(task, extra, deployment);
		}

		static void PrintHelp()
		{
			Console.WriteLine ("");
			Console.WriteLine("Djinn is a utility to aid infrastructure deployment across various cloud providers.");
			Console.WriteLine ("");
			Console.WriteLine("Install time: " + DjinnConfiguration.Instance.InstallDateTime.ToString() );
			
			Console.WriteLine ("");
			Console.WriteLine("Usage:\n  djinn <task> <deployment_specification> <machine_specification>");
			Console.WriteLine(string.Empty);
			
			using (var tw = new StringWriter() )
			{
				OptionSet.WriteOptionDescriptions(tw);
				Console.WriteLine("Options:\n" + tw.GetStringBuilder().ToString());
			}
			
			Console.WriteLine ("Actions:\n  " + string.Join("\n  ", Enum.GetNames(typeof(Task))).ToLower()+ "\n");
			
			Console.WriteLine("Deployments:");
			
			foreach (var deployment in DjinnEnvironment.Deployments)
			{
				Console.WriteLine(deployment.Identity);
				
				//var endpoints = CloudProvider.GetEndpoints(
				//	deployment.DeploymentGroup.Id,
				//	deployment.Blueprint.Id);
				
				//if( endpoints.Count != deployment.HorizontalScale )
				//{
				//	Console.WriteLine(string.Format("    - {0}/{1} endpoints", endpoints.Count, deployment.HorizontalScale ));
				//}
				//else
				//{
				//	endpoints.ForEach( x => Console.WriteLine("    - " + x + "\n"));
				//}
			}
			
			Console.WriteLine ("");

			CloudProvider.PrintStatus();
			
			Console.WriteLine ("");

		}
		

    }
}

