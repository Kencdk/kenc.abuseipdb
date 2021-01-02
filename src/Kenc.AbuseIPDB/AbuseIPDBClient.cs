namespace Kenc.AbuseIPDB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Kenc.AbuseIPDB.ApiReplies;
    using Kenc.AbuseIPDB.Entities;
    using Kenc.AbuseIPDB.Replies;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Implementation of <see cref="IAbuseIPDBClient"/> for interacting with the AbuseIPDB API.
    /// </summary>
    /// <inheritdoc/>
    public class AbuseIPDBClient : IAbuseIPDBClient
    {
        private readonly Uri baseUri;
        private readonly HttpClient httpClient;
        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="AbuseIPDBClient"/> class.
        /// </summary>
        /// <param name="httpClient">HttpClient to use for the connections.</param>
        /// <param name="options">Configurations for the client.</param>
        /// <exception cref="ArgumentNullException">Throws when any of the parameters are null or <see cref="string.Empty"/></exception>
        public AbuseIPDBClient(HttpClient httpClient, IOptions<AbuseIPDBClientSettings> options) : this(httpClient, options.Value.APIKey, options.Value.APIEndpoint)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbuseIPDBClient"/> class.
        /// </summary>
        /// <param name="httpClient">HttpClient to use for the connections.</param>
        /// <param name="apiKey">API key to use.</param>
        /// <param name="apiEndpoint">Endpoint to use.</param>
        /// <exception cref="ArgumentNullException">Throws when any of the parameters are null or <see cref="string.Empty"/></exception>
        public AbuseIPDBClient(HttpClient httpClient, string apiKey, Uri apiEndpoint)
        {
            _ = string.IsNullOrEmpty(apiKey) ? throw new ArgumentNullException(nameof(apiKey)) : apiKey;
            baseUri = apiEndpoint ?? throw new ArgumentNullException(nameof(apiEndpoint));

            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.httpClient.DefaultRequestHeaders.Add("Key", apiKey);
            this.httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<(IReadOnlyList<BlackListEntry> Data, BlackListMetadata Metadata, RateLimit RateLimit)> BlackListAsync(int confidenceMinimum = 100, int limit = 10000, CancellationToken cancellationToken = default)
        {
            var targetUri = new Uri(baseUri, $"blacklist?confidenceMinimum={confidenceMinimum}&limit={limit}");
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(targetUri, cancellationToken);

            return await HandleListResultAsync<BlackListEntry, BlackListMetadata>(httpResponseMessage, cancellationToken);
        }

        public async Task<(Check Data, RateLimit RateLimit)> CheckAsync(string ipAddress, int maxAgeInDays, bool verbose, CancellationToken cancellationToken)
        {
            var verboseParam = verbose ? "&verbose" : string.Empty;
            var targetUri = new Uri(baseUri, $"check?ipAddress={HttpUtility.UrlEncode(ipAddress)}&maxAgeInDays={maxAgeInDays}{verboseParam}");
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(targetUri, cancellationToken);

            return await HandleSingleResultAsync<Check>(httpResponseMessage, cancellationToken);
        }

        public async Task<(ReportUpdate Data, RateLimit rateLimit)> ReportAsync(string ip, string comment, Category[] categories, CancellationToken cancellationToken = default)
        {
            var categoryStr = string.Join(",", categories.Select(x => (int)x));
            var targetUri = new Uri(baseUri, $"report?ip={HttpUtility.UrlEncode(ip)}&categories={categoryStr}&comment={HttpUtility.UrlEncode(comment)}");
            HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(targetUri, null, cancellationToken);

            return await HandleSingleResultAsync<ReportUpdate>(httpResponseMessage, cancellationToken);
        }

        private async Task<(T Data, RateLimit RateLimit)> HandleResultAsync<T>(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken) where T : IApiReply
        {
            var rateLimit = RateLimit.FromHttpResponseHeaders(httpResponseMessage.Headers);
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                // an error occured; extract the errors and throw an exception.
                ErrorReply errorResponse = await httpResponseMessage.Content.ReadFromJsonAsync<ErrorReply>(jsonSerializerOptions, cancellationToken);
                throw new Exceptions.ApiException(httpResponseMessage.StatusCode, errorResponse.Errors, rateLimit);
            }

            T result = await httpResponseMessage.Content.ReadFromJsonAsync<T>(jsonSerializerOptions, cancellationToken);
            return (result, rateLimit);
        }

        private async Task<(T Data, M Metadata, RateLimit RateLimit)> HandleSingleResultAsync<T, M>(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
            where T : IApiEntity
            where M : IApiMetadata
        {
            (SingleEntryReply<T, M> data, RateLimit rateLimit) = await HandleResultAsync<SingleEntryReply<T, M>>(httpResponseMessage, cancellationToken);
            return (data.Data, data.Meta, rateLimit);
        }

        private async Task<(T Data, RateLimit RateLimit)> HandleSingleResultAsync<T>(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
            where T : IApiEntity
        {
            (SingleEntryReply<T, EmptyMetadata> data, RateLimit rateLimit) = await HandleResultAsync<SingleEntryReply<T, EmptyMetadata>>(httpResponseMessage, cancellationToken);
            return (data.Data, rateLimit);
        }

        private async Task<(IReadOnlyList<T> Data, M MetaData, RateLimit RateLimit)> HandleListResultAsync<T, M>(HttpResponseMessage httpResponseMessage, CancellationToken cancellationToken)
            where T : IApiEntity
            where M : IApiMetadata
        {
            (ListApiReply<T, M> data, RateLimit rateLimit) = await HandleResultAsync<ListApiReply<T, M>>(httpResponseMessage, cancellationToken);
            return (data.Data, data.Meta, rateLimit);
        }
    }
}
