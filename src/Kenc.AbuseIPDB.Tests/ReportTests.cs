namespace Kenc.AbuseIPDB.Tests
{
    using System;
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
    public class ReportTests
    {
        private const string okBody = @"{
    ""data"": {
      ""ipAddress"": ""127.0.0.1"",
      ""abuseConfidenceScore"": 52
    }
}";

        [TestMethod]
        public async Task CheckAddsParametersCorrectly()
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
            (ReportUpdate data, RateLimit rateLimit) = await abuseIpDbClient.ReportAsync("127.0.0.1", "Comment with spaces", new[] { Category.BadWebBot }, CancellationToken.None);

            sentRequestMessage.Should().NotBeNull();
            sentRequestMessage.RequestUri.AbsoluteUri.Should().Be("https://api.abuseipdb.com/api/v2/report?ip=127.0.0.1&categories=19&comment=Comment%20with%20spaces");
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
            (ReportUpdate data, RateLimit rateLimit) = await abuseIpDbClient.ReportAsync("127.0.0.1", "Comment with spaces", new[] { Category.BadWebBot }, CancellationToken.None);

            // check the result
            data.IpAddress.Should().Be("127.0.0.1");
            data.AbuseConfidenceScore.Should().Be(52);
        }
    }
}
