namespace Kenc.AbuseIPDB.Cache
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class DependencyInjection
    {
        /// <summary>
        /// Add <see cref="IAbuseIPDBClient"/> to dependency injection.
        /// </summary>
        /// <param name="serviceCollection">Service collection to add it to</param>
        /// <param name="clientConfiguration">Configuration to parse <see cref="AbuseIPDBClientSettings"/> from.</param>
        /// <param name="cacheConfiguration">Configuration to parse <see cref="CacheConfiguration"/> from.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAbuseIPDBClientCache(this IServiceCollection serviceCollection, IConfiguration clientConfiguration, IConfiguration cacheConfiguration)
        {
            return serviceCollection.AddSingleton<IAbuseIPDBClient, CachedAbuseIPDBClient>()
                .Configure<AbuseIPDBClientSettings>(clientConfiguration)
                .Configure<CacheConfiguration>(cacheConfiguration);
        }
    }
}
