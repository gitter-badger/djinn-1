using System;

namespace Sungiant.Djinn.Specification
{
    public class Rsync
        : IAction
    {
        public Rsync()
        {
            // Sensible defaults
            Delete = true;
            Verbose = true;
            Recursive = true;
            IsSourceContextRemote = false;
            IsDestinationContextRemote = true;
            Checksum = true;
        }

        // Base class data
        public String Type { get; set; }
        public String Description { get; set; }

        // RsyncActionSpecification data
        public Boolean Delete { get; set; }        // sets the --delete flag
        public Boolean Recursive { get; set; }     // sets the -r flag
        public Boolean Verbose { get; set; }       // sets the -v flag
        public Boolean Quiet { get; set; }         // sets the -q flag
        public Boolean Checksum { get; set; }      // sets the -c flag
        public Boolean Shortcuts { get; set; }     // sets the -l flag
        public Boolean Stats { get; set; }         // sets the --stats flag
        public String Source { get; set; }
        public Boolean IsSourceContextRemote { get; set; }
        public String Destination { get; set; }
        public Boolean IsDestinationContextRemote { get; set; }
    }
}

