namespace Kenc.AbuseIPDB.ApiReplies
{
    using System.Collections.Generic;

    public class ListApiReply<T, M> : IApiReply<IReadOnlyList<T>, M>
        where T : IApiEntity
        where M : IApiMetadata
    {
        public M Meta { get; set; }

        public IReadOnlyList<T> Data { get; set; }
    }
}
