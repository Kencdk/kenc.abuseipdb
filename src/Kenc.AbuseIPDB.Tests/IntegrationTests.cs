namespace Kenc.AbuseIPDB.Tests
{
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Kenc.AbuseIPDB.Exceptions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [TestCategory(TestConstants.IntegrationTests)]
    public class IntegrationTests
    {
        private static IAbuseIPDBClient client;
        private static TestContext context;

        [ClassInitialize]
        public static void ClassInitialiser(TestContext context)
        {
            IntegrationTests.context = context;
            var apiKey = (string)context.Properties["apikey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new System.Exception("API Key not specified. Cannot run tests");
            }
            client = new AbuseIPDBClient(httpClient: new HttpClient(), apiKey, AbuseIpDbEndpoints.V2Endpoint);
        }

        [TestMethod]
        public async Task HandleErrorsCorrectly()
        {
            static async Task fdas() =>
                await client.ReportAsync("127.0.0.2", "Validation testing report endpoint", new[] { Category.BadWebBot });

            ApiException exception = await Assert.ThrowsExceptionAsync<ApiException>(fdas);

            ApiReplies.Error firstError = exception.Errors[0];
            firstError.Status.Should().Be(HttpStatusCode.Forbidden);
            firstError.Source.Parameter.Should().Be("ip");
            firstError.Detail.Should().Be("You can only report the same IP address (`127.0.0.2`) once in 15 minutes.");
        }

        [TestMethod]
        public async Task DoIPLookup()
        {
            var ip = (string)context.Properties["ipaddress"];
            (Entities.Check data, RateLimit _) = await client.CheckAsync(ip, 90, false, CancellationToken.None);

            data.IpAddress.Should().Be(ip);
            data.CountryCode.Should().Be("US");
        }
    }
}
