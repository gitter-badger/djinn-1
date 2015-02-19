using System;
using System.Collections.Generic;
using System.Linq;

namespace Sungiant.Djinn
{
    public class Zone
    {
        readonly String identifier;
        readonly List<Deployment> deployments;
        
        public Zone(Specification.Zone spec, Dictionary<String, Blueprint> blueprintObjects)
        {
            this.identifier = spec.ZoneIdentifier;

            deployments = spec.Deployments
                .Select (x => new Deployment (x, this, blueprintObjects[x.BlueprintIdentifier]))
                .ToList ();
        }
        
        public String Identifier { get { return identifier; } }

        public List<Deployment> Deployments { get { return deployments; } }
    }
}

