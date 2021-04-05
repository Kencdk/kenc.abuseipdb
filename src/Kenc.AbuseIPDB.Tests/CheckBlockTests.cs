namespace Kenc.AbuseIPDB.Tests
{
    using System;
    using System.Linq;
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
    public class CheckBlockTests
    {
        [TestMethod]
        public async Task ResponseIsDeserializedAsExpected()
        {
            using var content = new StringContent(@"{
    ""data"": {
      ""networkAddress"": ""127.0.0.0"",
      ""netmask"": ""255.255.255.0"",
      ""minAddress"": ""127.0.0.1"",
      ""maxAddress"": ""127.0.0.254"",
      ""numPossibleHosts"": 254,
      ""addressSpaceDesc"": ""Loopback"",
      ""reportedAddress"": [
        {
          ""ipAddress"": ""127.0.0.1"",
          ""numReports"": 631,
          ""mostRecentReport"": ""2019-03-21T16:35:16+00:00"",
          ""abuseConfidenceScore"": 0,
          ""countryCode"": null
        },
        {
          ""ipAddress"": ""127.0.0.2"",
          ""numReports"": 16,
          ""mostRecentReport"": ""2019-03-12T20:31:17+00:00"",
          ""abuseConfidenceScore"": 0,
          ""countryCode"": null
        },
        {
          ""ipAddress"": ""127.0.0.3"",
          ""numReports"": 17,
          ""mostRecentReport"": ""2019-03-12T20:31:44+00:00"",
          ""abuseConfidenceScore"": 0,
          ""countryCode"": null
        }
      ]
    }
}");
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
            (CheckBlockData data, RateLimit rateLimit) = await abuseIpDbClient.CheckBlockAsync("127.0.0.1/24", 30, CancellationToken.None);

            // check the result
            data.NetworkAddress.Should().Be("127.0.0.0");
            data.Netmask.Should().Be("255.255.255.0");
            data.MinAddress.Should().Be("127.0.0.1");
            data.MaxAddress.Should().Be("127.0.0.254");
            data.NumPossibleHosts.Should().Be(254);
            data.AddressSpaceDesc.Should().Be("Loopback");
            data.ReportedAddress.Count.Should().Be(3);

            CheckBlockEntry[] reportedAddresses = data.ReportedAddress.ToArray();
            reportedAddresses[0].IPAddress.Should().Be("127.0.0.1");
            reportedAddresses[0].NumReports.Should().Be(631);
            reportedAddresses[0].MostRecentReport.Should().Be(new DateTimeOffset(2019, 03, 21, 16, 35, 16, 0, TimeSpan.Zero));
            reportedAddresses[0].AbuseConfidenceScore.Should().Be(0);
            reportedAddresses[0].CountryCode.Should().BeNull();
        }
    }
}
