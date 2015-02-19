using System;
using System.Collections.Generic;
using Amazon.EC2.Model;
using System.Linq;

namespace Sungiant.Cloud.Azure
{
    internal class AzureCloudDeployment
    : ICloudDeployment
    {
        internal AzureCloudDeployment(CloudDeploymentIdentity identifier)
        {
            Identifier = identifier;
        }
        
        public Int32 VerticleScale 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }
        
        public Int32 HorizontalScale 
        { 
            get
            {
                throw new NotImplementedException();
            }
        }
        
        public List<String> Endpoints
        { 
            get
            {
                throw new NotImplementedException();
            }
        }
        
        public CloudDeploymentIdentity Identifier { get; private set; }
        
        public CloudDeploymentStatus Status
        { 
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}

