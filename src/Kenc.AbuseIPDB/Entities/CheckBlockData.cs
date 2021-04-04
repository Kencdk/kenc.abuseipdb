namespace Kenc.AbuseIPDB.Entities
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using Kenc.AbuseIPDB.ApiReplies;

    public class CheckBlockData : IApiEntity
    {
        [JsonPropertyName("networkAddress")]
        public string NetworkAddress { get; set; }

        [JsonPropertyName("netmask")]
        public string Netmask { get; set; }

        [JsonPropertyName("minAddress")]
        public string MinAddress { get; set; }

        [JsonPropertyName("maxAddress")]
        public string MaxAddress { get; set; }

        [JsonPropertyName("numPossibleHosts")]
        public int NumPossibleHosts { get; set; }

        [JsonPropertyName("addressSpaceDesc")]
        public string AddressSpaceDesc { get; set; }

        [JsonPropertyName("reportedAddress")]
        public IReadOnlyList<CheckBlockEntry> ReportedAddress { get; set; }
    }
}
