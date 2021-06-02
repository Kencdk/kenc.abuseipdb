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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Moq.Protected;

    [TestClass]
    public class BlacklistTests
    {
        private const string okBody = @"{
  ""meta"": {
    ""generatedAt"": ""2020-09-24T19:54:11+00:00""
  },
  ""data"": [
    {
      ""ipAddress"": ""5.188.10.179"",
      ""abuseConfidenceScore"": 100,
      ""lastReportedAt"": ""2020-09-24T19:17:02+00:00""
    },
    {
      ""ipAddress"": ""185.222.209.14"",
      ""abuseConfidenceScore"": 100,
      ""lastReportedAt"": ""2020-09-24T19:17:02+00:00""
    },
    {
      ""ipAddress"": ""191.96.249.183"",
      ""abuseConfidenceScore"": 100,
      ""lastReportedAt"": ""2020-09-24T19:17:01+00:00""
    }
  ]
}";

        [TestMethod]
        public async Task BlacklistAddsParametersCorrectly()
        {
            using var content = new StringContent(okBody);
            using var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = content,
            };

            HttpRequestMessage sentRequestMessage = null;

            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .Callback((HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) =>
               {
                   sentRequestMessage = httpRequestMessage;
               })
               .ReturnsAsync(response);

            var httpClient = new HttpClient(httpMessageHandler.Object);
            var abuseIpDbClient = new AbuseIPDBClient(httpClient, "apiKey", new Uri("https://api.abuseipdb.com/api/v2/"));
            (IReadOnlyList<BlackListEntry> Data, Replies.BlackListMetadata Metadata, RateLimit RateLimit) = await abuseIpDbClient.BlackListAsync(90, 1000, CancellationToken.None);

            sentRequestMessage.Should().NotBeNull();
            sentRequestMessage.RequestUri.AbsoluteUri.Should().Be("https://api.abuseipdb.com/api/v2/blacklist?confidenceMinimum=90&limit=1000");
        }

        [TestMethod]
        public async Task BlacklistAddsParametersWithDefaultValues()
        {
            using var content = new StringContent(okBody);
            using var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = content,
            };

            HttpRequestMessage sentRequestMessage = null;

            var httpMessageHandler = new Mock<HttpMessageHandler>();
            httpMessageHandler
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .Callback((HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken) =>
               {
                   sentRequestMessage = httpRequestMessage;
               })
               .ReturnsAsync(response);

            var httpClient = new HttpClient(httpMessageHandler.Object);
            var abuseIpDbClient = new AbuseIPDBClient(httpClient, "apiKey", new Uri("https://api.abuseipdb.com/api/v2/"));
            (IReadOnlyList<BlackListEntry> Data, Replies.BlackListMetadata Metadata, RateLimit RateLimit) = await abuseIpDbClient.BlackListAsync(cancellationToken: CancellationToken.None);

            sentRequestMessage.Should().NotBeNull();
            sentRequestMessage.RequestUri.AbsoluteUri.Should().Be("https://api.abuseipdb.com/api/v2/blacklist?confidenceMinimum=100&limit=10000");
        }

        [TestMethod]
        public async Task ResponseIsDeserializedAsExpected()
        {
            using var content = new StringContent(okBody);
            using var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
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
            (IReadOnlyList<BlackListEntry> data, Replies.BlackListMetadata Metadata, RateLimit _) = await abuseIpDbClient.BlackListAsync(cancellationToken: CancellationToken.None);

            Metadata.GeneratedAt.Should().Be(DateTimeOffset.Parse("2020-09-24T19:54:11+00:00"));

            /*  ""meta"": {
    ""generatedAt"": ""2020-09-24T19:54:11+00:00""
  },
  ""data"": [
    {
      ""ipAddress"": ""5.188.10.179"",
      ""abuseConfidenceScore"": 100,
      ""lastReportedAt"": ""2020-09-24T19:17:02+00:00""
    },
    {
      ""ipAddress"": ""185.222.209.14"",
      ""abuseConfidenceScore"": 100,
      ""lastReportedAt"": ""2020-09-24T19:17:02+00:00""
    },
    {
      ""ipAddress"": ""191.96.249.183"",
      ""abuseConfidenceScore"": 100,
      ""lastReportedAt"": ""2020-09-24T19:17:01+00:00""
    }
             * */
        }
    }
}
