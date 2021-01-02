namespace Kenc.AbuseIPDB.ApiReplies
{
    using System.Text.Json.Serialization;

    public class SingleEntryReply<T> : IApiReply
        where T : IApiEntity
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }
    }

    public class SingleEntryReply<T, M> : IApiReply
        where T : IApiEntity
        where M : IApiMetadata
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }

        [JsonPropertyName("meta")]
        public M Meta { get; set; }
    }
}
