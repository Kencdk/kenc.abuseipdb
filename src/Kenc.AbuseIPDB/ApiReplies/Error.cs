namespace Kenc.AbuseIPDB.ApiReplies
{
    using System.Net;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Class wrapping an error in the AbuseIPDB API.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Gets or sets the detail parameter.
        /// </summary>
        [JsonPropertyName("detail")]
        public string Detail { get; set; }

        /// <summary>
        /// Gets or sets the status code parameter.
        /// </summary>
        [JsonPropertyName("status")]
        public HttpStatusCode Status { get; set; }

        /// <summary>
        /// Gets or sets the source parameter.
        /// </summary>
        [JsonPropertyName("source")]
        public ErrorSource Source { get; set; }
    }

    public class ErrorSource
    {
        [JsonPropertyName("parameter")]
        public string Parameter { get; set; }
    }
}
