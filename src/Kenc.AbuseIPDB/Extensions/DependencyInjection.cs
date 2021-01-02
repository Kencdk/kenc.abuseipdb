namespace Kenc.AbuseIPDB
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class DependencyInjection
    {
        public static IServiceCollection AddAbuseIPDBClient(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            return serviceCollection.AddSingleton<IAbuseIPDBClient, AbuseIPDBClient>()
                .Configure<AbuseIPDBClientSettings>(configuration);
        }
    }
}
