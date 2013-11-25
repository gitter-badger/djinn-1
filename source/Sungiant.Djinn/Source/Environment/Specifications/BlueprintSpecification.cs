using System;
using System.Collections.Generic;

namespace Sungiant.Djinn
{
	/// <summary>
	/// A BlueprintSpecification reperesents a set of 
	/// instructions for provisioning a machine.
	/// </summary>
	public class BlueprintSpecification
	{
		/// <summary>
		/// A string identifier to represent this blueprint.
		/// </summary>
		public String BlueprintId { get; set; }

		/// <summary>
		/// A human readable description of the purpose of
		/// this blueprint.
		/// </summary>
		public String Description { get; set; }

		/// <summary>
		/// Which ports need to be open for this machine.
		/// </summary>
		public List<Int32> OpenPorts { get; set; }

		/// <summary>
		/// Defines what needs to be done to configure the server.
		/// </summary>
		public List<ActionGroup> Configure { get; set; }

		/// <summary>
		/// Defines what needs to be done to deploy 
		/// code / dlls / apps to the server.
		/// </summary>
		public List<ActionGroup> Deploy { get; set; }
	}
}

