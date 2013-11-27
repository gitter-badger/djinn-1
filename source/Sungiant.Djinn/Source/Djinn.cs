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
	internal class JsonTypeObject
	{
		public String Type { get; set; }
	}

	static class Djinn
    {
		public static DjinnEnvironment DjinnEnvironment { get; private set; }

		static Int32 LogLevel { get; set; }
		static Boolean ShowHelp { get; set; }

		static ICloudProvider CloudProvider;

		static OptionSet OptionSet;

		const string Version = "0.2.3";

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

		readonly static List<String> supportedExtensions = new List<String>(){ ".xml", ".json" };

		public static List<T> LoadSpecifications<T>(String directory) where T : class, new()
		{
			var result = Directory.GetFiles (directory)
			    // filter out unsupported file types
				.Where (x => supportedExtensions.Contains(Path.GetExtension(x).ToLower()))
				// strip off the file name
				.Select (x => Path.GetFileNameWithoutExtension (x))
				// select distinct filenames, incase there are files of 
				// the same name but with different supported extensions
				.Distinct ()
				// select the first extension that exists
				.Select (x => x + supportedExtensions.Find(y => File.Exists(Path.Combine(directory, x + y))))
				// deserialise the chosen file
				.Select(x => FromFile<T>(Path.Combine(directory, x)))
				// filter out acceptable error case
				.Where(x => x != null)
				// and return
				.ToList();

			return result;
		}

		public static T FromFile<T>(String file) where T : class, new()
		{
			if (!File.Exists (file))
				throw new Exception ("Failed to find file: " + file);

			String allText = file.ReadAllText ();

			if (string.IsNullOrWhiteSpace (allText))
			{
				return null;
			}

			T spec = null;
			String ext = Path.GetExtension (file).ToLower();

			if (ext == supportedExtensions[0])
			{
				spec = allText.FromXml<T> ();
			}
			else if (ext == supportedExtensions[1])
			{
				spec = allText.FromJson<T> ();
			}

			if (spec == null)
				throw new Exception ("Failed to deserialize file: " + file);

			return spec;
		}

		static void ConfigureCustomJsonDeserialization()
		{
			JsConfig<Specification.INginxLocationBlock>.RawDeserializeFn = (String s) =>
			{
				var d = s.FromJson<JsonTypeObject>();

				var t = Type.GetType("Sungiant.Djinn.Specification." + d.Type + ", Sungiant.Djinn.Specification");

				Object o = JsonSerializer.DeserializeFromString(s, t);

				return (o as Specification.INginxLocationBlock);
			};

			JsConfig<Specification.IAction>.RawDeserializeFn = (String s) =>
			{
				var d = s.FromJson<JsonTypeObject>();

				var t = Type.GetType("Sungiant.Djinn.Specification." + d.Type + ", Sungiant.Djinn.Specification");

				Object o = JsonSerializer.DeserializeFromString(s, t);

				return (o as Specification.IAction);
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

			ConfigureCustomJsonDeserialization ();

			// loads up djinn's configuration file
			DjinnConfiguration.Instance.Load ();

			InitiliseCloudProvider();

			Console.WriteLine ("Active Workgroup: " + DjinnConfiguration.Instance.ActiveWorkgroup.WorkgroupIdentifier);

			var environmentSetupData = new EnvironmentSetupData (DjinnConfiguration.Instance.ActiveWorkgroup.WorkgroupIdentifier);

			foreach (var projectConfig in DjinnConfiguration.Instance.ActiveWorkgroup.ProjectConfigurations)
			{
				var blueprint_specs = LoadSpecifications<Specification.Blueprint> (projectConfig.BlueprintsDirectory);
				var deployment_specs = LoadSpecifications<Specification.Deployment> (projectConfig.DeploymentsDirectory);
				var zone_specs = LoadSpecifications<Specification.Zone> (projectConfig.ZonesDirectory);

				environmentSetupData.AddProject (
					projectConfig.DjinnDirectory,
					blueprint_specs,
					deployment_specs,
					zone_specs
				);
			}

			DjinnEnvironment = new DjinnEnvironment(environmentSetupData);

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
			
			String task = args[0];
			String deploymentGroupIdentifier = args[1];
			String machineBlueprintIdentifier = args[2];
			String extra = null;

			if( args.Length == 4 )
			{
				extra = args[3];
			}

			OptionSet.Parse (args);

			// make sure the deployment we want to talk to exists
			var deployment = DjinnEnvironment.Projects
				.SelectMany(x => x.Deployments)
				.ToList()
				.Find (x => (x.DeploymentGroup.Identifier == deploymentGroupIdentifier && x.Blueprint.Identifier == machineBlueprintIdentifier));

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


			foreach (var project in DjinnEnvironment.Projects)
			{
				Console.WriteLine("Project: " + project.LocalContext );
				Console.WriteLine ("");
				
				foreach (var deployment in project.Deployments)
				{
					Console.WriteLine("    + " + deployment.Identity);
					
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
			}

			Console.WriteLine ("");
			Console.WriteLine ("Cloud Provider Status:");

			CloudProvider.PrintStatus();
			
			Console.WriteLine ("");

		}
		

    }
}

