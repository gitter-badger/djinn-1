using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ServiceStack.Text;

using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;

namespace Sungiant.Cloud.Aws
{
    public static class AwsEc2Helper
    {
        public static string GetAwsSize(int verticleScale)
        {
            switch(verticleScale)
            {
                case 1: return "t1.micro";
                case 2: return "m1.small";
                case 3: return "m1.medium";
                case 4: return "m1.large";
                default: throw new NotImplementedException();
            }
        }

        public static void PrintStatus(AmazonEC2 client)
        {
            var allDeployments = SelectAllCloudDeployments(client);

            if (allDeployments != null)
            {
                if (allDeployments.Count > 0)
                {
                    Console.WriteLine ("Cloud deployments");

                    allDeployments
                    .ToList ()
                    .ForEach (x => Console.WriteLine (" - " + x.Identifier.IdentiferTags.First ().Value + System.Environment.NewLine + x.Endpoints.Join (System.Environment.NewLine + "   - ")));
                    return;
                }
        
                Console.WriteLine ("Zero EC2 Instances");
            }
        }
        
        // Amazon Provided Ubuntu Cloud Guest AMI - Ubuntu Server 12.10
        const string ImageID = "ami-7539b41c";
        
        public static void DestroyDeployment(
            AmazonEC2 client,
            CloudDeploymentIdentity identity
            )
        {
            var aCd = Describe(client, identity);
            
            if( aCd != null )
            {
                var response = client.TerminateInstances(
                    new TerminateInstancesRequest()
                    {
                        InstanceId = aCd.RunningInstances.Select(x => x.InstanceId).ToList()
                    }
                );

                response.TerminateInstancesResult.PrintDump();
            
            }
            else
            {
                throw new Exception("deployment doesn't exist");
            }
            
        }
        
        public static void GetCloudDeployment(
            AmazonEC2 client,
            CloudDeploymentIdentity identity,
            String keyName,
            String securityGroupName,
            Int32 horizontalScale,
            Int32 verticalScale)
        {
            Console.WriteLine("==> Launching {0} instances (image {1}, key {2})", horizontalScale, ImageID, keyName);

            RunInstancesRequest runInstances = new RunInstancesRequest()
            {
                ImageId = ImageID,
                InstanceType = GetAwsSize(verticalScale),
                MinCount = horizontalScale,
                MaxCount = horizontalScale,
                KeyName = keyName
            };

            runInstances.SecurityGroup.Add(securityGroupName);
            
            var runResponse = client.RunInstances(runInstances);
            
            List<string> instanceIDs = runResponse.RunInstancesResult.Reservation.RunningInstance
                .Select(instance => instance.InstanceId)
                .ToList();

            Console.WriteLine("==> Launched {0}", instanceIDs.Join(", "));
            
            if (instanceIDs.Count != horizontalScale)
                throw new Exception("==> Failed to launch the correct number of instances");


            Console.WriteLine("==> Tagging instances");
            
            List<Tag> djinnTags = GetDjinnAwsTags(identity);
            
            client.CreateTags(
                new CreateTagsRequest()
                {
                ResourceId = instanceIDs,
                Tag = djinnTags
            }
            );
            
            // name each
            for (int i = 0; i < instanceIDs.Count; ++i)
            {
                client.CreateTags(
                    new CreateTagsRequest()
                    {
                    ResourceId = new List<string>() 
                    { 
                        instanceIDs[i] 
                    },
                    Tag = new List<Tag>()
                    { 
                        new Tag() 
                        { 
                            Key = "Name", 
                            Value = string.Format("{0} #{1}", identity.IdentiferTags.First().Value, i + 1) 
                        }
                    }
                }
                );  
            }

            Console.WriteLine("==> Waiting for instances to spin up");
            List<RunningInstance> instances = null;

            Console.WriteLine(String.Format("==> Querying status of {0} instances", instanceIDs.Count));

            while (true)
            {
                try
                {
                    DescribeInstancesResponse statusResponse = client.DescribeInstances(
                        new DescribeInstancesRequest()
                        {
                            InstanceId = instanceIDs
                        }
                    );
                    
                    instances = statusResponse.DescribeInstancesResult.Reservation
                        .SelectMany(reservation => reservation.RunningInstance).ToList();


                    int numRunning = instances.Count(inst => inst.InstanceState.Name == "running");
                    if (numRunning == instances.Count)
                    {
                        Console.WriteLine(String.Format("==> All {0} instances are now running.", instanceIDs.Count));
                        break;
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                
                Console.WriteLine();
                Thread.Sleep(4000);
            }

            Console.WriteLine("==> Waiting for ssh to start");
            List<String> dnsNames = instances.Select(inst => inst.PublicDnsName).ToList();

            foreach (String dns in dnsNames)
            {
                Console.WriteLine(String.Format("==> Checking port 22 of {0}", dns));
                while (true)
                {
                    var output = new List<String> ();

                    ProcessHelper.Run (String.Format("nc -zv {0} 22", dns), output.Add);

                    if (output.Count > 0 && output.Any(x => x.Contains("succeeded")))
                    {
                        break;
                    }

                    Thread.Sleep (10);
                }
            }
            
            Console.WriteLine("==> Ssh is up.");
        }
        
        const Int32 MaxNumAwsTags = 10;
        const Int32 MaxNumDjinnTags = MaxNumAwsTags - 1;
        const Int32 AwsMaxTagLength = 255;
        
        
        static List<Tag> GetDjinnAwsTags(CloudDeploymentIdentity identity)
        {
            List<Tag> tags = new List<Tag>(MaxNumDjinnTags);

            string jsonIdentity = JsonSerializer.SerializeToString(identity);
            
            byte[] bytes = jsonIdentity.ToUtf8Bytes();
            
            string base64 = Convert.ToBase64String(bytes);
            
            int numCharacters = jsonIdentity.Length;
            
            int maxIdentitySize = AwsMaxTagLength * MaxNumDjinnTags;
            
            if( maxIdentitySize < numCharacters )
            {
                throw new Exception("Cloud Deployment Identity is too large to be encoded in AWS instace tags");
            }
            
            int counter = 0;
            
            string workString = base64;
            string testString = string.Empty;
            
            while( workString.Length > 0 )
            {
                string thisPart;
                
                if( workString.Length > AwsMaxTagLength )
                {
                    // take the first part
                    thisPart = workString.Substring(0, AwsMaxTagLength);
                    
                    // remove 
                    workString = workString.Substring(AwsMaxTagLength, workString.Length - AwsMaxTagLength);
                }
                else
                {
                    thisPart = workString;
                    workString = string.Empty;
                }
                
                tags.Add(
                    new Tag()
                    {
                        Key = DjinnTag + counter,
                        Value = thisPart
                    }
                );
                
                testString += thisPart;
            
                counter++;
            }
            
            string decodedTest = new System.Text.UTF8Encoding().GetString(Convert.FromBase64String(testString));
            
            if( decodedTest != jsonIdentity )
            {
                throw new Exception("Identity tag encoding problem!");
            }
            
            return tags;
        }
        
        const string DjinnTag = "Djinn";
        
        internal static AwsCloudDeployment Describe(AmazonEC2 client, CloudDeploymentIdentity identity)
        {
            var allCloudDeployments = SelectAllCloudDeployments(client);
            
            var result = allCloudDeployments.Find(x => x.Identifier == identity);
                
            return result;
        }
        
        static AwsCloudDeployment GetCloudDeployment(CloudDeploymentIdentity identity, List<RunningInstance> runningInstances)
        {
            //if( runningInstances.All(x => GetIdentity(x) == identity) )
            //{
                var cd = new AwsCloudDeployment(identity, runningInstances);
                
                return cd;
            //}
            //else
            //{
            //  throw new Exception("bad data");
            //}
        }
        
        static CloudDeploymentIdentity GetIdentity(RunningInstance instance)
        {
            var djinnTags = instance.Tag
                .Where(x => x.Key.Contains(DjinnTag))
                .ToList();
            
            var sortedKeys = djinnTags.Select(x => x.Key).ToList();
            
            sortedKeys.Sort();
            
            var base64Identity = string.Empty;
            
            sortedKeys.ForEach(x => base64Identity += djinnTags.Find(y => y.Key == x).Value);
            

            if( base64Identity.Length == 0 )
            {
                throw new Exception(string.Format("{0} tag is missing.", DjinnTag));
            }
            
            try
            {
                byte[] bytes = Convert.FromBase64String(base64Identity);
            
                var encoding = new System.Text.UTF8Encoding();
                
                var json = encoding.GetString(bytes);
                
                var identity = json.FromJson<CloudDeploymentIdentity>();
            
                return identity;
            }
            catch
            {
                throw new Exception(string.Format("{0} tag is corrupt.", DjinnTag));
            }
        }
        
        static List<AwsCloudDeployment> SelectAllCloudDeployments(AmazonEC2 client)
        {
            Console.WriteLine("AWS EC2 Instances:");
            DescribeInstancesResponse response = null;

            try
            {
                response = client.DescribeInstances(new DescribeInstancesRequest());
            }
            catch(System.Net.WebException e)
            {
                Console.WriteLine("🔥  Failed to Describe AWS EC2 Instances: " + e.GetType() + " - " + e.Message);
                return null;
            }

            var result1 = response.DescribeInstancesResult.Reservation
                .SelectMany (a => a.RunningInstance)
                .Where (b => b.InstanceState.Name != "terminated")
                .Where (b => b.InstanceState.Name != "shutting-down")
                .ToList();
                
            var result2 = result1
                .GroupBy (GetIdentity).ToList();
            
            
            var result3 = result2
                .Select (x => GetCloudDeployment(x.Key, x.ToList()))
                .ToList();
                
            return result3;
            
        }
    }
}

