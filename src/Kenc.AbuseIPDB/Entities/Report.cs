namespace Kenc.AbuseIPDB.Entities
{
    using System;
    using System.Text.Json.Serialization;

    public class Report
    {
        [JsonPropertyName("reportedAt")]
        public DateTimeOffset ReportedAt { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        [JsonPropertyName("categories")]
        public Category[] Categories { get; set; }

        [JsonPropertyName("reporterId")]
        public int ReporterId { get; set; }

        [JsonPropertyName("reporterCountryCode")]
        public string ReporterCountryCode { get; set; }

        [JsonPropertyName("reporterCountryName")]
        public string ReporterCountryName { get; set; }
    }
}