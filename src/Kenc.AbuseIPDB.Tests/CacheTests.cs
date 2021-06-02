namespace Kenc.AbuseIPDB.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Kenc.AbuseIPDB.Entities;
    using LazyCache;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Options;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class CacheTests
    {
        [TestMethod]
        public async Task SanityCheck()
        {
            var innerClient = new Mock<IAbuseIPDBClient>();
            innerClient.Setup(x => x.CheckAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((new Check(), new RateLimit(0, 1000, 999, DateTimeOffset.UtcNow.AddHours(1))));

            innerClient.Setup(x => x.CheckBlockAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((new CheckBlockData(), new RateLimit(0, 1000, 999, DateTimeOffset.UtcNow.AddHours(1))));

            var configuration = new Cache.CacheConfiguration();
            var instance = new Cache.CachedAbuseIPDBClient(innerClient.Object, Options.Create(configuration));

            await instance.CheckAsync("127.0.0.1", 30, true, CancellationToken.None);
            await instance.CheckBlockAsync("127.0.0.1/24", 30, CancellationToken.None);
        }

        [TestMethod]
        public async Task CheckAsyncUsesRightCacheKey()
        {
            var innerClient = new Mock<IAbuseIPDBClient>();
            innerClient.Setup(x => x.CheckAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((new Check(), new RateLimit(0, 1000, 999, DateTimeOffset.UtcNow.AddHours(1))));

            var configuration = new Cache.CacheConfiguration();
            var cache = new Mock<IAppCache>();
            var instance = new Cache.CachedAbuseIPDBClient(innerClient.Object, Options.Create(configuration), cache.Object);

            await instance.CheckAsync("127.0.0.1", 30, true, CancellationToken.None);

            cache.Verify(x => x.GetOrAddAsync("127.0.0.1", It.IsAny<Func<ICacheEntry, Task<Check>>>()), Times.Once);
        }

        [TestMethod]
        public async Task CheckBlockAsyncUsesRightCacheKey()
        {
            var innerClient = new Mock<IAbuseIPDBClient>();
            innerClient.Setup(x => x.CheckBlockAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((new CheckBlockData(), new RateLimit(0, 1000, 999, DateTimeOffset.UtcNow.AddHours(1))));

            var configuration = new Cache.CacheConfiguration();
            var cache = new Mock<IAppCache>();
            var instance = new Cache.CachedAbuseIPDBClient(innerClient.Object, Options.Create(configuration), cache.Object);

            await instance.CheckBlockAsync("127.0.0.1/24", 30, CancellationToken.None);

            cache.Verify(x => x.GetOrAddAsync("127.0.0.1/24", It.IsAny<Func<ICacheEntry, Task<CheckBlockData>>>()), Times.Once);
        }

        [TestMethod]
        public async Task CheckAsyncCallsClientIfNotInCache()
        {
            var innerClient = new Mock<IAbuseIPDBClient>();
            innerClient.Setup(x => x.CheckAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((new Check(), new RateLimit(0, 1000, 999, DateTimeOffset.UtcNow.AddHours(1))));

            var cache = new LazyCache.Mocks.MockCachingService();

            var configuration = new Cache.CacheConfiguration();
            var instance = new Cache.CachedAbuseIPDBClient(innerClient.Object, Options.Create(configuration), cache);

            await instance.CheckAsync("127.0.0.1", 30, true, CancellationToken.None);
            innerClient.Verify(x => x.CheckAsync("127.0.0.1", 30, true, It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task CheckBlockAsyncCallsClientIfNotInCache()
        {
            var innerClient = new Mock<IAbuseIPDBClient>();
            innerClient.Setup(x => x.CheckBlockAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((new CheckBlockData(), new RateLimit(0, 1000, 999, DateTimeOffset.UtcNow.AddHours(1))));

            var cache = new LazyCache.Mocks.MockCachingService();

            var configuration = new Cache.CacheConfiguration();
            var instance = new Cache.CachedAbuseIPDBClient(innerClient.Object, Options.Create(configuration), cache);

            await instance.CheckBlockAsync("127.0.0.1/24", 30, CancellationToken.None);
            innerClient.Verify(x => x.CheckBlockAsync("127.0.0.1/24", 30, It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task ReportBypassesCache()
        {
            var innerClient = new Mock<IAbuseIPDBClient>();
            innerClient.Setup(x => x.ReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Category[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((new ReportUpdate(), new RateLimit(0, 1000, 999, DateTimeOffset.UtcNow.AddHours(1))));

            var cache = new Mock<IAppCache>();
            var configuration = new Cache.CacheConfiguration();
            var instance = new Cache.CachedAbuseIPDBClient(innerClient.Object, Options.Create(configuration), cache.Object);

            await instance.ReportAsync("127.0.0.1", "Comment", new[] { Category.BadWebBot }, CancellationToken.None);
            cache.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task BlackListUsesCache()
        {
            IReadOnlyList<BlackListEntry> blackList = new[]
            {
                new BlackListEntry()
            }.ToList();

            var innerClient = new Mock<IAbuseIPDBClient>();
            innerClient.Setup(x => x.BlackListAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((blackList, new Replies.BlackListMetadata(), new RateLimit(0, 1000, 999, DateTimeOffset.UtcNow.AddHours(1))));

            var cache = new CachingService();
            var configuration = new Cache.CacheConfiguration();
            var instance = new Cache.CachedAbuseIPDBClient(innerClient.Object, Options.Create(configuration), cache);

            await instance.BlackListAsync();

            (IReadOnlyList<BlackListEntry> data, Replies.BlackListMetadata metadata) = cache.Get<(IReadOnlyList<BlackListEntry> data, Replies.BlackListMetadata metadata)>("blacklist_100_10000");
            data.Should().BeSameAs(blackList);
        }
    }
}
