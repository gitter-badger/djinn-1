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
    public static class AwsEc2SecurityGroupHelper
    {
        public static void Create(
            AmazonEC2 client,
            CloudSecurityGroup securityGroup)
        {
            var response = client.CreateSecurityGroup( 
                new CreateSecurityGroupRequest()
                {
                    GroupName = securityGroup.Name,
                    GroupDescription = securityGroup.Description
                }
            );
            
            string groupId = response.CreateSecurityGroupResult.GroupId;
            
            foreach( var item in securityGroup.Rules )
            {
                client.AuthorizeSecurityGroupIngress(
                    new AuthorizeSecurityGroupIngressRequest()
                    {
                        GroupId = groupId,
                        FromPort = item.Port,
                        ToPort = item.Port,
                        IpProtocol = item.Mode.ToString().ToLower(),
                        CidrIp = "0.0.0.0/0"
                    }
                );
            }
        }

        public static Boolean Exists(AmazonEC2 client, String securityGroupName)
        {
            try
            {
                var result = client.DescribeSecurityGroups(
                    new DescribeSecurityGroupsRequest()
                    {
                        GroupName = new List<String> () { securityGroupName }
                    }
                );
                
                var sg = 
                    result.DescribeSecurityGroupsResult.SecurityGroup
                        .Find(x => x.GroupName == securityGroupName);

                if( sg != null )
                    return true;
                    
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static Boolean Matches(
            AmazonEC2 client,
            CloudSecurityGroup securityGroup)
        {
            var result = client.DescribeSecurityGroups(
                new DescribeSecurityGroupsRequest()
                {
                    GroupName = new List<String> () { securityGroup.Name }
                }
            );

            if( result.DescribeSecurityGroupsResult.IsSetSecurityGroup() )
            {
                var sg = 
                    result.DescribeSecurityGroupsResult.SecurityGroup
                        .Find(x => x.GroupName == securityGroup.Name);

                if( sg != null )
                {
                    if( securityGroup.Rules.Count != sg.IpPermission.Count )
                        return false;

                    foreach(var ipp in sg.IpPermission)
                    {
                        if( securityGroup.Rules.Find(x => 
                              x.Mode.ToString().ToLower() == ipp.IpProtocol &&
                              x.Port == ipp.FromPort &&
                              x.Port == ipp.ToPort) == null )
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        public static Boolean InUse(AmazonEC2 client, String securityGroupName)
        {
            var response = client.DescribeInstances(new DescribeInstancesRequest());

            Int32 intanceCount = response.DescribeInstancesResult.Reservation
                .SelectMany(a => a.RunningInstance)
                .Where(b => b.InstanceState.Name != "terminated")
                .SelectMany(c => c.GroupName)
                .Where(d => d == securityGroupName)
                .ToList()
                .Count();

            return intanceCount > 0;
        }

        public static void Destroy(AmazonEC2 client, String securityGroupName)
        {
            client.DeleteSecurityGroup(
                new DeleteSecurityGroupRequest()
                {
                    GroupName = securityGroupName
                }
            );
        }

        public static void PrintStatus(AmazonEC2 client)
        {
            Console.WriteLine("AWS EC2 Security Groups:");

            DescribeSecurityGroupsResponse result = null;
            try
            {
                result = client.DescribeSecurityGroups( new DescribeSecurityGroupsRequest() );
            }
            catch(System.Net.WebException e)
            {
                Console.WriteLine("🔥  Failed to describe AWS EC2 Security Groups: " + e.GetType() + " - " + e.Message);
                return;
            }
            
            if( result.DescribeSecurityGroupsResult.IsSetSecurityGroup() )
            {


                if( result.DescribeSecurityGroupsResult.SecurityGroup.Count > 0 )
                {
                    result
                        .DescribeSecurityGroupsResult
                        .SecurityGroup
                        .ForEach( x => Console.WriteLine(" - " + x.GroupName));

                    return;
                }
            }

            Console.WriteLine("Zero Aws Security Groups");
        }
    }
}

