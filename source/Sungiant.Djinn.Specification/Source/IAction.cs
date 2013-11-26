using System;

namespace Sungiant.Djinn.Specification
{
	public interface IAction
	{
		String Type { get; set; }
		String Description { get; set; }
	}
}

