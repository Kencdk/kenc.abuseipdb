namespace Kenc.AbuseIPDB.Cache
{
    using System;

    /// <summary>
    /// Configuration of Abuse IP DB cache.
    /// </summary>
    public class CacheConfiguration
    {
        /// <summary>
        /// Gets or sets how long to cache the results for Check operations.
        /// </summary>
        public TimeSpan CheckCacheLifetime { get; set; } = TimeSpan.FromMinutes(60);

        /// <summary>
        /// Gets or sets how long to cache the results for CheckBlock operations.
        /// </summary>
        public TimeSpan CheckBlockCacheLifetime { get; set; } = TimeSpan.FromMinutes(60);

        /// <summary>
        /// Gets or sets how long to cache the results for blacklists.
        /// </summary>
        public TimeSpan BlackListCacheLifetime { get; set; } = TimeSpan.FromHours(24);
    }
}
