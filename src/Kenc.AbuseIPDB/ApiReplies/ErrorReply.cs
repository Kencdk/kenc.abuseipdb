namespace Kenc.AbuseIPDB.ApiReplies
{
    using System.Text.Json.Serialization;

    public class ErrorReply
    {
        [JsonPropertyName("errors")]
        public Error[] Errors { get; set; }
    }
}
