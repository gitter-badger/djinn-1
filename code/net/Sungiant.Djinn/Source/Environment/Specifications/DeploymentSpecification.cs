using System;

namespace Sungiant.Djinn
{
	public class DeploymentSpecification
	{
		// fk
		public String DeploymentGroupId { get; set; }

		// fk
		public String MachineBlueprintId { get; set; }

		public Int32 HorizontalScale { get; set; }

		public Int32 VerticalScale { get; set; }
	}


}

