namespace Kenc.AbuseIPDB
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Kenc.AbuseIPDB.Entities;
    using Kenc.AbuseIPDB.Replies;

    /// <summary>
    /// Interface for a client interacting with the Cloudflare API.
    /// </summary>
    public interface IAbuseIPDBClient
    {
        Task<(Check Data, RateLimit RateLimit)> CheckAsync(string ipAddress, int maxAgeInDays, bool verbose, CancellationToken cancellationToken);

        /// <summary>
        /// The blacklist is the culmination of all of the valiant reporting by AbuseIPDB users. It's a list of the most reported IP addresses.
        /// </summary>
        /// <param name="confidenceMinimum">Minimum confidence rating. https://www.abuseipdb.com/faq.html#confidence</param>
        /// <param name="limit">To conserve bandwidth, the number of IP addresses included in the list is capped to 10,000. Subscribers, both basic and premium, can overcome this limit. All users can set it between 1 and 10,000 using the limit parameter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><see cref="Results.BlackListReply"/></returns>
        Task<(IReadOnlyList<BlackListEntry> Data, BlackListMetadata Metadata, RateLimit RateLimit)> BlackListAsync(int confidenceMinimum = 100, int limit = 10000, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reporting IP addresses is the core feature of AbuseIPDB.
        /// When you report what you observe, everyone benefits, including yourself.
        /// Related details can be included in the comment field.
        /// </summary>
        /// <param name="ip">IP address</param>
        /// <param name="comment">Comment to include related information.</param>
        /// <param name="categories">Categories of the report.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        /// <remarks>STRIP ANY PERSONALLY IDENTIFIABLE INFORMATION (PII); AbuseIPDB IS NOT RESPONSIBLE FOR PII YOU REVEAL.</remarks>
        Task<(ReportUpdate Data, RateLimit rateLimit)> ReportAsync(string ip, string comment, Category[] categories, CancellationToken cancellationToken = default);

        /// <summary>
        /// The check-block endpoint accepts a subnet (v4 or v6) denoted with CIDR notation.
        /// The maxAgeInDays parameter determines how old the reports considered in the query search can be.
        /// The desired data is stored in the data property.Here you can inspect details regarding the network queried, such as the netmask of the subnet, the number of hosts it can possibly contain, and the assigned description of the address space.
        /// The network should be url-encoded, because the network parameter contains a forward slash, which is a reserved character in URIs.
        /// </summary>
        /// <param name="ipBlock">CIDR notation of either an IPv4 or IPv6 subnet.</param>
        /// <param name="maxAgeInDays">Max number of days.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        Task<(CheckBlockData data, RateLimit rateLimit)> CheckBlockAsync(string ipBlock, int maxAgeInDays = 30, CancellationToken cancellationToken = default);
    }
}
