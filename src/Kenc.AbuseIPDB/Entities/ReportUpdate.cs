namespace Kenc.AbuseIPDB.Entities
{
    using System.Text.Json.Serialization;
    using Kenc.AbuseIPDB.ApiReplies;

    public class ReportUpdate : IApiEntity
    {
        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("abuseConfidenceScore")]
        public int AbuseConfidenceScore { get; set; }
    }
}
