using System;
using System.Collections.Generic;
using System.Linq;

namespace Sungiant.Djinn
{
	public class DjinnEnvironment
	{
		DjinnEnvironmentSpecification Spec { get; set; }

		public DjinnEnvironment(DjinnEnvironmentSpecification spec)
		{
			Spec = spec;

			MachineBlueprints = new Dictionary<string, MachineBlueprint>();

			Spec.MachineBlueprintSpecifications
				.Select(x => new MachineBlueprint(x))
				.ToList()
				.ForEach(x => MachineBlueprints.Add(x.Id, x));

			DeploymentGroups = new Dictionary<string, DeploymentGroup>();

			Spec.DeploymentGroupSpecifications
				.Select(x => new DeploymentGroup(x))
				.ToList()
				.ForEach(x => DeploymentGroups.Add(x.Id, x));

			Deployments = Spec.DeploymentSpecifications
				.Select(x => new Deployment(x, DeploymentGroups, MachineBlueprints))
				.ToList();
		}

		public List<Deployment> Deployments { get; private set; }
		public Dictionary<String, DeploymentGroup> DeploymentGroups { get; private set; }
		public Dictionary<String, MachineBlueprint> MachineBlueprints { get; private set; }


	}
}

