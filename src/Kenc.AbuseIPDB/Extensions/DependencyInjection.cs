namespace Kenc.AbuseIPDB
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class DependencyInjection
    {
        /// <summary>
        /// Add <see cref="IAbuseIPDBClient"/> to dependency injection.
        /// </summary>
        /// <param name="serviceCollection">Service collection to add it to</param>
        /// <param name="configuration">Configuration to parse <see cref="AbuseIPDBClientSettings"/> from.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAbuseIPDBClient(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            return serviceCollection.AddSingleton<IAbuseIPDBClient, AbuseIPDBClient>()
                .Configure<AbuseIPDBClientSettings>(configuration);
        }
    }
}
