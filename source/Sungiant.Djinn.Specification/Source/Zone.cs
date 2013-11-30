using System;

namespace Sungiant.Djinn.Specification
{
	/// <summary>
	/// A ZoneSpecification defines a zone for housing deployments.
	/// </summary>
	public class Zone
	{
		/// <summary>
		/// A string identifier to represent this zone
		/// </summary>
		public String ZoneIdentifier { get; set; }

		/// <summary>
		/// A human readable description of the purpose of
		/// this zone.
		/// </summary>
		public Deployment[] Deployments { get; set; }

	}
}

