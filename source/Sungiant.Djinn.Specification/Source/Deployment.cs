using System;
using System.Collections.Generic;

namespace Sungiant.Djinn.Specification
{
	/// <summary>
	/// DeploymentSpecification defines an instance
	/// of a Blueprint in a Zone.
	/// </summary>
	public class Deployment
	{
		/// <summary>
		/// Which Zone group should be used.
		/// </summary>
		public String ZoneIdentifier { get; set; }

		/// <summary>
		/// Which Blueprint should be used.
		/// </summary>
		public String BlueprintIdentifier { get; set; }

		/// <summary>
		/// How many many machines?
		/// </summary>
		public Int32 HorizontalScale { get; set; }

		/// <summary>
		/// What size machines?
		/// </summary>
		public Int32 VerticalScale { get; set; }

		public Dictionary<String, String> EnvironmentVariables { get; set; }
	}
}

