using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;
using System.Text;
using Sungiant.Cloud;

namespace Sungiant.Djinn.Tasks
{
    public class Provision
        : Task
    {
        public Provision (ICloudProvider cloudProvider, Deployment deployment)
            : base (TaskType.Provision, cloudProvider, deployment) {}

        public override void Run(Boolean dryRun)
        {
            var cp = CloudProvider.Describe(Deployment.Identity);
            
            if (cp == null)
            {
                CloudProvider.Launch(
                    Deployment.Identity,
                    Deployment.Blueprint.Security,
                    Deployment.VerticalScale,
                    Deployment.HorizontalScale
                    );
            }
        }
    }
}

