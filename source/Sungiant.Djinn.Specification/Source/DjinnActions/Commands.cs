using System;

namespace Sungiant.Djinn.Specification
{
    public class Commands
        : IAction
    {
        // Base class data
        public String Type { get; set; }
        public String Description { get; set; }

        // CommandSpecification data
        public Boolean IsContextRemote { get; set; }
        public String[] Values { get; set; }
        public Boolean IgnoreFailure { get; set; }
    }
}

