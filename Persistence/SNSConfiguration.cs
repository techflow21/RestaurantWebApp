
using Amazon;

namespace Persistence
{
    public class SNSConfiguration
    {
        public string AwsKeyId { get; set; }
        public string AwsKeySecret { get; set; }
        public RegionEndpoint RegionEndpoint { get; set; }
    }
}
