using System;

namespace Sungiant.Djinn
{
	/// <summary>
	/// A ZoneSpecification defines a zone for housing deployments.
	/// </summary>
	public class ZoneSpecification
	{
		/// <summary>
		/// A string identifier to represent this zone
		/// </summary>
		public String ZoneId { get; set; }

		/// <summary>
		/// A human readable description of the purpose of
		/// this zone.
		/// </summary>
		public String Description { get; set; }

	}
}

