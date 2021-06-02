namespace Kenc.AbuseIPDB.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Kenc.AbuseIPDB.Entities;
    using Kenc.AbuseIPDB.Replies;
    using LazyCache;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Cache layer around <see cref="IAbuseIPDBClient"/>.
    /// </summary>
    /// <inheritdoc/>
    public class CachedAbuseIPDBClient : IAbuseIPDBClient
    {
        private readonly IAppCache cache;
        private readonly IAbuseIPDBClient client;
        private readonly IOptions<CacheConfiguration> cacheConfiguration;
        private readonly ReaderWriterLockSlim rwl = new();

        private RateLimit latestCheckRateLimit;
        private RateLimit latestBlackListRateLimit;
        private RateLimit latestCheckBlockRateLimit;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedAbuseIPDBClient"/> class.
        /// </summary>
        /// <param name="client">Client to use for outgoing connections.</param>
        /// <param name="cacheConfiguration">Configuration of the cache.</param>
        /// <param name="cache">Instance of IAppCache.</param>
        internal CachedAbuseIPDBClient(IAbuseIPDBClient client, IOptions<CacheConfiguration> cacheConfiguration, IAppCache cache = null)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.cache = cache ?? new CachingService();

            if (cacheConfiguration.Value == null)
            {
                throw new ArgumentNullException(nameof(cacheConfiguration));
            }

            this.cacheConfiguration = cacheConfiguration;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedAbuseIPDBClient"/> class.
        /// </summary>
        /// <param name="httpClient">HTTPClient to use for connections.</param>
        /// <param name="clientConfiguration">Configuration of <see cref="AbuseIPDBClient"/>.</param>
        /// <param name="cacheConfiguration">Configuration of cache.</param>
        /// <param name="cache">Instance of IAppCache.</param>
        public CachedAbuseIPDBClient(HttpClient httpClient, IOptions<AbuseIPDBClientSettings> clientConfiguration, IOptions<CacheConfiguration> cacheConfiguration, IAppCache cache = null)
        {
            this.cache = cache ?? new CachingService();
            if (cacheConfiguration.Value == null)
            {
                throw new ArgumentNullException(nameof(cacheConfiguration));
            }

            this.cacheConfiguration = cacheConfiguration;
            client = new AbuseIPDBClient(httpClient, clientConfiguration);
        }

        public async Task<(IReadOnlyList<BlackListEntry> Data, BlackListMetadata Metadata, RateLimit RateLimit)> BlackListAsync(int confidenceMinimum = 100, int limit = 10000, CancellationToken cancellationToken = default)
        {
            (IReadOnlyList<BlackListEntry> data, BlackListMetadata metadata) = await cache.GetOrAddAsync($"blacklist_{confidenceMinimum}_{limit}", async (item) =>
            {
                item.AbsoluteExpirationRelativeToNow = cacheConfiguration.Value.BlackListCacheLifetime;
                (IReadOnlyList<BlackListEntry> data, BlackListMetadata metadata, RateLimit rateLimit) = await client.BlackListAsync(confidenceMinimum, limit, cancellationToken);

                if (rwl.TryEnterWriteLock(1000))
                {
                    try
                    {
                        latestBlackListRateLimit = rateLimit;
                    }
                    finally
                    {
                        rwl.ExitWriteLock();
                    }
                }

                return (data, metadata);
            });


            if (rwl.TryEnterReadLock(1000))
            {
                try
                {
                    return (data, metadata, latestBlackListRateLimit);
                }
                finally
                {
                    rwl.ExitReadLock();
                }
            }

            return (data, metadata, null);
        }

        public async Task<(Check Data, RateLimit RateLimit)> CheckAsync(string ipAddress, int maxAgeInDays, bool verbose, CancellationToken cancellationToken)
        {
            Check data = await cache.GetOrAddAsync(ipAddress, async (item) =>
            {
                item.AbsoluteExpirationRelativeToNow = cacheConfiguration.Value.CheckCacheLifetime;
                (Check Data, RateLimit rateLimit) = await client.CheckAsync(ipAddress, maxAgeInDays, verbose, cancellationToken);

                if (rwl.TryEnterWriteLock(1000))
                {
                    try
                    {
                        latestCheckRateLimit = rateLimit;
                    }
                    finally
                    {
                        rwl.ExitWriteLock();
                    }
                }

                return Data;
            });


            if (rwl.TryEnterReadLock(1000))
            {
                try
                {
                    return (data, latestCheckRateLimit);
                }
                finally
                {
                    rwl.ExitReadLock();
                }
            }

            return (data, null);
        }

        public async Task<(CheckBlockData data, RateLimit rateLimit)> CheckBlockAsync(string ipBlock, int maxAgeInDays = 30, CancellationToken cancellationToken = default)
        {
            CheckBlockData data = await cache.GetOrAddAsync(ipBlock, async (item) =>
            {
                item.AbsoluteExpirationRelativeToNow = cacheConfiguration.Value.CheckBlockCacheLifetime;
                (CheckBlockData Data, RateLimit rateLimit) = await client.CheckBlockAsync(ipBlock, maxAgeInDays, cancellationToken);

                if (rwl.TryEnterWriteLock(1000))
                {
                    try
                    {
                        latestCheckBlockRateLimit = rateLimit;
                    }
                    finally
                    {
                        rwl.ExitWriteLock();
                    }
                }
                return Data;
            });


            if (rwl.TryEnterReadLock(1000))
            {
                try
                {
                    return (data, latestCheckBlockRateLimit);
                }
                finally
                {
                    rwl.ExitReadLock();
                }
            }

            return (data, null);
        }

        public Task<(ReportUpdate Data, RateLimit rateLimit)> ReportAsync(string ip, string comment, Category[] categories, CancellationToken cancellationToken = default)
        {
            return client.ReportAsync(ip, comment, categories, cancellationToken);
        }
    }
}
