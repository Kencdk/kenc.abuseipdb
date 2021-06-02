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
    public class CheckTests
    {
        private const string okBody = @"{
    ""data"": {
      ""ipAddress"": ""118.25.6.39"",
      ""isPublic"": true,
      ""ipVersion"": 4,
      ""isWhitelisted"": false,
      ""abuseConfidenceScore"": 100,
      ""countryCode"": ""CN"",
      ""countryName"": ""China"",
      ""usageType"": ""Data Center/Web Hosting/Transit"",
      ""isp"": ""Tencent Cloud Computing (Beijing) Co. Ltd"",
      ""domain"": ""tencent.com"",
      ""hostnames"": [],
      ""totalReports"": 1,
      ""numDistinctUsers"": 1,
      ""lastReportedAt"": ""2018-12-20T20:55:14+00:00"",
      ""reports"": [
        {
          ""reportedAt"": ""2018-12-20T20:55:14+00:00"",
          ""comment"": ""Dec 20 20:55:14 srv206 sshd[13937]: Invalid user oracle from 118.25.6.39"",
          ""categories"": [
            18,
            22
          ],
          ""reporterId"": 1,
          ""reporterCountryCode"": ""US"",
          ""reporterCountryName"": ""United States""
        }
      ]
    }
}";

        [DataTestMethod]
        [DataRow(false, "https://api.abuseipdb.com/api/v2/check?ipAddress=127.0.0.1&maxAgeInDays=30")]
        [DataRow(true, "https://api.abuseipdb.com/api/v2/check?ipAddress=127.0.0.1&maxAgeInDays=30&verbose")]
        public async Task CheckAddsParametersCorrectly(bool verbose, string expectedUrl)
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
            (Check data, RateLimit rateLimit) = await abuseIpDbClient.CheckAsync("127.0.0.1", 30, verbose, CancellationToken.None);

            sentRequestMessage.Should().NotBeNull();
            sentRequestMessage.RequestUri.AbsoluteUri.Should().Be(expectedUrl);
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
            (Check data, RateLimit rateLimit) = await abuseIpDbClient.CheckAsync("118.25.6.39", 30, true, CancellationToken.None);

            // check the result
            data.IpAddress.Should().Be("118.25.6.39");
            data.IsPublic.Should().BeTrue();
            data.IPVersion.Should().Be(4);
            data.IsWhitelisted.Should().BeFalse();
            data.AbuseConfidenceScore.Should().Be(100);
            data.CountryCode.Should().Be("CN");
            data.CountryName.Should().Be("China");
            data.UsageType.Should().Be("Data Center/Web Hosting/Transit");
            data.ISP.Should().Be("Tencent Cloud Computing (Beijing) Co. Ltd");
            data.Domain.Should().Be("tencent.com");
            data.Hostnames.Should().BeEmpty();
            data.TotalReports.Should().Be(1);
            data.NumberOfDistinctUsers.Should().Be(1);
            data.LastReportedAt.Should().Be(DateTimeOffset.Parse("2018-12-20T20:55:14+00:00"));
            data.Reports.Should().HaveCount(1);

            Report report = data.Reports[0];
            report.Should().NotBeNull();
            report.ReportedAt.Should().Be(DateTimeOffset.Parse("2018-12-20T20:55:14+00:00"));
            report.Comment.Should().Be("Dec 20 20:55:14 srv206 sshd[13937]: Invalid user oracle from 118.25.6.39");
            report.Categories.Should().HaveCount(2);
            report.Categories.Should().BeEquivalentTo(new[] { Category.BruteForce, Category.SSH });
            report.ReporterId.Should().Be(1);
            report.ReporterCountryCode.Should().Be("US");
            report.ReporterCountryName.Should().Be("United States");
        }
    }
}
