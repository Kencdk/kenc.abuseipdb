namespace Kenc.AbuseIPDB.ApiReplies
{
    public interface IApiReply
    {
    }

    public interface IApiReply<T, M> : IApiReply
        where M : IApiMetadata
    {
        M Meta { get; set; }

        T Data { get; set; }
    }
}
