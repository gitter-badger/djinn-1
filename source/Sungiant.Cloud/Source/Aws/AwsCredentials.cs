using System;

namespace Sungiant.Cloud.Aws
{
    public class AwsCredentials
    {
        public String AccountNumber { get; set; }
        public String AccessKeyId { get; set; }
        public String SecretKey { get; set; }

        public String PrivateKeyPath { get; set; }
        public String PrivateKeyName { get; set; }
    }
}

