namespace Kenc.AbuseIPDB.Replies
{
    using System;
    using System.Text.Json.Serialization;
    using Kenc.AbuseIPDB.ApiReplies;

    public class BlackListMetadata : IApiMetadata
    {
        [JsonPropertyName("generatedAt")]
        public DateTimeOffset GeneratedAt { get; set; }
    }
}
