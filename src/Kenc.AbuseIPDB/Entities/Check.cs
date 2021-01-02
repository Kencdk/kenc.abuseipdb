namespace Kenc.AbuseIPDB.Entities
{
    using System;
    using System.Text.Json.Serialization;
    using Kenc.AbuseIPDB.ApiReplies;

    public class Check : IApiEntity
    {
        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("isPublic")]
        public bool IsPublic { get; set; }

        [JsonPropertyName("ipVersion")]
        public uint IPVersion { get; set; }

        [JsonPropertyName("isWhitelisted")]
        public bool? IsWhitelisted { get; set; }

        [JsonPropertyName("abuseConfidenceScore")]
        public int AbuseConfidenceScore { get; set; }

        [JsonPropertyName("countryCode")]
        public string CountryCode { get; set; }

        [JsonPropertyName("countryName")]
        public string CountryName { get; set; }

        [JsonPropertyName("usageType")]
        public string UsageType { get; set; }

        [JsonPropertyName("isp")]
        public string ISP { get; set; }

        [JsonPropertyName("hostnames")]
        public string[] Hostnames { get; set; }

        [JsonPropertyName("totalReports")]
        public uint TotalReports { get; set; }

        [JsonPropertyName("numDistinctUsers")]
        public uint NumberOfDistinctUsers { get; set; }

        [JsonPropertyName("lastReportedAt")]
        public DateTimeOffset? LastReportedAt { get; set; }

        [JsonPropertyName("reports")]
        public Report[] Reports { get; set; }
    }
}
