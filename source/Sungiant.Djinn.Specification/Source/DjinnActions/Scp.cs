using System;

namespace Sungiant.Djinn.Specification
{
    public class Scp
        : IAction
    {
        // Base class data
        public String Type { get; set; }
        public String Description { get; set; }

        // ScpSpecification data
        public Boolean Recursive { get; set; } // sets the -r flag
        public Boolean Verbose { get; set; } // sets the -v flag
        public Boolean Quiet { get; set; } // sets the -q flag
        public String Source { get; set; }
        public Boolean IsSourceContextRemote { get; set; }
        public String Destination { get; set; }
        public Boolean IsDestinationContextRemote { get; set; }
    }
}

