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
			Console.WriteLine ("Djinn v0.2.0");

			// loads up djinn's configuration file
			var configuration = DjinnConfiguration.Load ();

			DjinnConfiguration.Create(configuration);


			InitiliseCloudProvider();

			var machine_blueprint_specs = 
				Directory
					.GetFiles (configuration.ActiveWorkgroup.MachineBlueprintSpecificationsDirectory)
					.Select(x => x.ReadAllText())
					.Select(x => x.FromXml<MachineBlueprintSpecification>())
					.ToList();

			var deployment_specs = 
				Directory
					.GetFiles (configuration.ActiveWorkgroup.DeploymentSpecificationsDirectory)
					.Select(x => x.ReadAllText())
					.Select(x => x.FromXml<DeploymentSpecification>())
					.ToList();

			var deployment_group_specs = 
				Directory
					.GetFiles (configuration.ActiveWorkgroup.DeploymentGroupSpecificationsDirectory)
					.Select(x => x.ReadAllText())
					.Select(x => x.FromXml<DeploymentGroupSpecification>())
					.ToList();
		
			var environmentSpecification = new DjinnEnvironmentSpecification () {
				MachineBlueprintSpecifications = machine_blueprint_specs,
				DeploymentSpecifications = deployment_specs,
				DeploymentGroupSpecifications = deployment_group_specs,
			};

			DjinnEnvironment = new DjinnEnvironment(environmentSpecification);

			var djinnTask = ParseArguments(args);

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
			if (DjinnConfiguration.Instance.AwsCredentials != null && DjinnConfiguration.Instance.AzureCredentials != null)
			{
				throw new NotImplementedException("todo: do you want to use azure or aws?");
			}
			
			if (DjinnConfiguration.Instance.AwsCredentials != null)
			{
				CloudProvider = new Sungiant.Cloud.Aws.Aws(DjinnConfiguration.Instance.AwsCredentials);
			}
			
			if (DjinnConfiguration.Instance.AzureCredentials != null)
			{
				CloudProvider = new Sungiant.Cloud.Azure.Azure(DjinnConfiguration.Instance.AzureCredentials);
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
				.Find (x => (x.DeploymentGroup.Id == deploymentGroupId && x.MachineBlueprint.Id == machineBlueprintId));

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
				//	deployment.MachineBlueprint.Id);
				
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

