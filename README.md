# kenc.abuseipdb
Wrapper around abuseipdb.com API for .net

## Getting started

```Powershell
PM> Install-Package Kenc.AbuseIPDB
```

Kenc.AbuseIPDb is built with dependency-injection as a first-class-citizen.
As a result, there's a helper function to register the library including pointing to the configuration section, if IConfiguration is being utilized.

```C#
services.AddAbuseIPDBClient(Configuration.GetSection("AbuseIPDB"));
```

with the configuration section "AbuseIPDB" having the following settings:
```JSON
  "AbuseIPDB": {
    "APIKey": "<APIKey>",
    "APIEndpoint": "https://api.abuseipdb.com/api/v2/"
  }
```

_Note_: Don't embed your API key with your source code, load it from [keyvault](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-5.0) or another secure storage.

### Using the client
With the client registered with dependency injection, add IAbuseIPDBClient abuseIPDBClient to your constructor, eg:
```C#
private readonly ILogger<HomeController> _logger;
private readonly IAbuseIPDBClient _abuseIPDBClient;
private readonly IHttpContextAccessor _httpContextAccessor;

public HomeController(ILogger<HomeController> logger, IAbuseIPDBClient abuseIPDBClient, IHttpContextAccessor httpContextAccessor)
{
    _logger = logger;
    _abuseIPDBClient = abuseIPDBClient ?? throw new ArgumentNullException(nameof(abuseIPDBClient));
    _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
}
```

In an ASP.net MVC app, to check for an abusive IP:
```C#
[HttpPost]
public async Task<IActionResult> PostComment(CommentModel comment)
{
    var ip = _httpContextAccessor.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress;
    try
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        (Check abuseCheck, _) = await _abuseIPDBClient.CheckAsync(ip.ToString(), 90, false, cancellationTokenSource.Token);

        if (abuseCheck.AbuseConfidenceScore > 70)
        {
            _logger.LogWarning($"{nameof(HomeController)} refused comment by {ip} due to high abuse confidence score.");
            var error = new ErrorModel
            {
                Title = "Bad Request.",
                Detail = "User IP is blocked due to abuse.",
                Instance = $"/comment/{Activity.Current?.Id ?? httpContextAccessor.HttpContext.TraceIdentifier}",
                Status = 400,
                Type = "/comment/abusiveip"
            };

            // return a 400 with the error information
            return Unauthorized(error);
        }
    }
    catch (OperationCanceledException)
    {
        _logger.LogError($"{nameof(HomeController)}: Failed to check AbuseIPDb within configured timeout.");
    }
    catch (ApiException abuseIPException)
    {
        _logger.LogError($"{nameof(HomeController)}: Caught error with AbuseIPDB: {abuseIPException.Message}");
    }
}
```

## Cached client

In case you have a website with a significant amount of traffic, or where users are expected to send multiple requests, consider using the cached IAbuseIPDBClient. This adds a memory cache in-front, so only a single lookup per IP/block is made.

```Powershell
PM> Install-Package Kenc.AbuseIPDB
```

Register it with dependency injection using:

```C#
services.AddAbuseIPDBClientCache(Configuration.GetSection("AbuseIPDB"), Configuration.GetSection("AbuseIPDBCache"));
```

The configuration section can be used to configure for how long values are cached.

| Name                    | Default  |
| ----------------------- | -------- |
| CheckCacheLifetime      | 1 hour   |
| CheckBlockCacheLifetime | 1 hour   |
| BlackListCacheLifetime  | 24 hours |

Configuration values are deserialized into TimeSpan. By default these follow [ISO 8601 durations](https://en.wikipedia.org/wiki/ISO_8601#Durations)

The following sets the configurations to keep Check() responses for 1 minute, CheckBlock() responses for 2 hours and 30 minutes and lastly BlackList() checks for 36 hours.
```json
{
    "AbuseIPDBCache": {
        "CheckCacheLifetime": "PT1M",
        "CheckBlockCacheLifetime": "PT2H30M",
        "BlackListCacheLifetime": "PT36H"
    }
}
```