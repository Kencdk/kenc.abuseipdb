namespace Kenc.AbuseIPDB.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Kenc.AbuseIPDB.Entities;
    using Kenc.AbuseIPDB.Exceptions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Moq.Protected;

    [TestClass]
    public class AbuseIpDbClientTests
    {
        [TestMethod]
        public async Task ReadsRateLimitHeader()
        {
            using var content = new StringContent("{}");
            using var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = content,
            };

            response.Headers.Add("Retry-After", "100");
            response.Headers.Add("X-RateLimit-Limit", "1000");
            response.Headers.Add("X-RateLimit-Remaining", "100");
            response.Headers.Add("X-RateLimit-Reset", "1545973200");

            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            var httpClient = new HttpClient(httpMessageHandler.Object);
            var abuseIpDbClient = new AbuseIPDBClient(httpClient, "apiKey", new Uri("https://api.abuseipdb.com/api/v2/"));
            (CheckBlockData data, RateLimit rateLimit) = await abuseIpDbClient.CheckBlockAsync("127.0.0.1/24", 30, CancellationToken.None);

            rateLimit.Limit.Should().Be(1000);
            rateLimit.Remaining.Should().Be(100);
            rateLimit.Reset.Should().Be(new DateTimeOffset(2018, 12, 28, 5, 0, 0, TimeSpan.Zero));
            rateLimit.RetryAfter.Should().Be(100);
        }

        [TestMethod]
        public async Task ReadsErrorResponse()
        {
            using var content = new StringContent(@"{
  ""errors"": [
      {
          ""detail"": ""Daily rate limit of 1000 requests exceeded for this endpoint. See headers for additional details."",
          ""status"": 429
      }
  ]
}");

            using var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.TooManyRequests,
                Content = content,
            };

            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            var httpClient = new HttpClient(httpMessageHandler.Object);
            var abuseIpDbClient = new AbuseIPDBClient(httpClient, "apiKey", new Uri("https://api.abuseipdb.com/api/v2/"));

            async Task action() => await abuseIpDbClient.CheckBlockAsync("127.0.0.1/24", 30, CancellationToken.None);
            ApiException exception = await Assert.ThrowsExceptionAsync<ApiException>(action);

            ApiReplies.Error firstError = exception.Errors[0];
            firstError.Status.Should().Be(HttpStatusCode.TooManyRequests);
            firstError.Detail.Should().Be("Daily rate limit of 1000 requests exceeded for this endpoint. See headers for additional details.");
        }

        [TestMethod]
        public void DependencyInjectionRegistersAsExpected()
        {
            var myConfiguration = new Dictionary<string, string>
            {
                { "APIKey", "Value1" },
                { "APIEndpoint", AbuseIpDbEndpoints.V2Endpoint.AbsoluteUri },
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddHttpClient();
            serviceCollection.AddAbuseIPDBClient(configuration);
            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            IAbuseIPDBClient resolvedService = serviceProvider.GetService<IAbuseIPDBClient>();
            resolvedService.Should().NotBeNull();
        }

        [TestMethod]
        public void V2EndpointIsAsExpected()
        {
            AbuseIpDbEndpoints.V2Endpoint.AbsoluteUri.Should().Be("https://api.abuseipdb.com/api/v2/");
        }
    }
}
