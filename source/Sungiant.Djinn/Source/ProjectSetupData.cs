using System;
using System.Collections.Generic;

namespace Sungiant.Djinn
{
    public class ProjectSetupData
    {
        public String LocalContext { get; set; }

        public List<Specification.Blueprint> BlueprintSpecifications { get; set; }
        public List<Specification.Zone> ZoneSpecifications { get; set; }
    }
}

