using System;

namespace Sungiant.Djinn.Specification
{
	public class ActionGroupContainer
	{
		public ActionGroup[] ActionGroups { get; set; }
	}

	/// <summary>
	/// A BlueprintSpecification reperesents a set of 
	/// instructions for provisioning a machine.
	/// </summary>
	public class Blueprint
	{
		/// <summary>
		/// A string identifier to represent this blueprint.
		/// </summary>
		public String BlueprintIdentifier { get; set; }

		/// <summary>
		/// A human readable description of the purpose of
		/// this blueprint.
		/// </summary>
		public String Description { get; set; }

		/// <summary>
		/// Which ports need to be open for this machine.
		/// </summary>
		public Int32[] OpenPorts { get; set; }

		/// <summary>
		/// Defines what needs to be done to configure the server.
		/// </summary>
		public ActionGroupContainer Configuration { get; set; }

		/// <summary>
		/// Defines what needs to be done to deploy 
		/// code / dlls / apps to the server.
		/// </summary>
		public ActionGroupContainer Deployment { get; set; }
	}
}

