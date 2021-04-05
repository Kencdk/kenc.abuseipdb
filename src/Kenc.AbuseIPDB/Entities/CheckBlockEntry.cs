namespace Kenc.AbuseIPDB.Entities
{
    using System;
    using System.Text.Json.Serialization;

    public class CheckBlockEntry
    {
        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }

        [JsonPropertyName("numReports")]
        public int NumReports { get; set; }

        [JsonPropertyName("mostRecentReport")]
        public DateTimeOffset MostRecentReport { get; set; }

        [JsonPropertyName("abuseConfidenceScore")]
        public int AbuseConfidenceScore { get; set; }

        [JsonPropertyName("countryCode")]
        public string CountryCode { get; set; }
    }
}
