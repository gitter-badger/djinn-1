using System;

namespace Sungiant.Djinn
{
	public class Command
	{
		// From what machine is this command run
		public MachineContext MachineContext { get; set; }

		// The command to run.
		public String Value { get; set; }

		// Should ignore errors?
		public Boolean IgnoreErrors { get; set; }
	}
}

