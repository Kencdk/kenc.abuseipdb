namespace Kenc.AbuseIPDB.ApiReplies
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class ListApiReply<T, M> : IApiReply<IReadOnlyList<T>, M>
        where T : IApiEntity
        where M : IApiMetadata
    {
        [JsonPropertyName("meta")]
        public M Meta { get; set; }

        [JsonPropertyName("data")]
        public IReadOnlyList<T> Data { get; set; }
    }
}
