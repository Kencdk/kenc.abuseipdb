namespace Kenc.AbuseIPDB.ApiReplies
{
    using System.Net;
    using System.Text.Json.Serialization;

    public class Error
    {
        [JsonPropertyName("detail")]
        public string Detail { get; set; }

        [JsonPropertyName("status")]
        public HttpStatusCode Status { get; set; }

        [JsonPropertyName("source")]
        public ErrorSource Source { get; set; }
    }

    public class ErrorSource
    {
        [JsonPropertyName("parameter")]
        public string Parameter { get; set; }
    }
}
