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
        /// <remarks> STRIP ANY PERSONALLY IDENTIFIABLE INFORMATION (PPI); AbuseIPDB IS NOT RESPONSIBLE FOR PPI YOU REVEAL.</remarks>
        Task<(ReportUpdate Data, RateLimit rateLimit)> ReportAsync(string ip, string comment, Category[] categories, CancellationToken cancellationToken = default);
    }
}
