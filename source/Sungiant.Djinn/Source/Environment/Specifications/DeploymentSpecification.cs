using System;

namespace Sungiant.Djinn
{
	/// <summary>
	/// DeploymentSpecification defines an instance
	/// of a Blueprint in a Zone.
	/// </summary>
	public class DeploymentSpecification
	{
		/// <summary>
		/// Which Zone group should be used.
		/// </summary>
		public String ZoneId { get; set; }

		/// <summary>
		/// Which Blueprint should be used.
		/// </summary>
		public String BlueprintId { get; set; }

		/// <summary>
		/// How many many machines?
		/// </summary>
		public Int32 HorizontalScale { get; set; }

		/// <summary>
		/// What size machines?
		/// </summary>
		public Int32 VerticalScale { get; set; }
	}
}

