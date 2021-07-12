# About #
Kenc.AbuseIPDB is a fully featured client for connecting with the AbuseIPDB.com API v2.

# How to use #
Kenc.AbuseIPDb is built with dependency-injection as a first-class-citizen.
As a result, there's a helper function to register the library including pointing to the configuration section, if IConfiguration is being utilized.

Configuration:
```JSON
"AbuseIPDB": {
  "APIEndpoint": "https://api.abuseipdb.com/api/v2/",
  "APIKey": "<apikey>"
}
```

*API key should be loaded from a secure storage, such as keyvault to prevent leaks.*

```C#
services.AddAbuseIPDBClient(Configuration.GetSection("AbuseIPDB"));

...
public HomeController(ILogger<BlogController> logger, IAbuseIPDBClient abuseIPDBClient, IHttpContextAccessor httpContextAccessor)
```

To lookup the requester IP of a request in ASP.net and reject IPs with an abuse confidence score of 70 or higher:
```C#
var ip = httpContextAccessor.HttpContext.Features.Get<IHttpConnectionFeature>()?.RemoteIpAddress;
try
{
    // skip for local loopback
    if (!IPAddress.IsLoopback(ip))
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        (Check abuseCheck, _) = await abuseIPDBClient.CheckAsync(ip.ToString(), 90, false, cancellationTokenSource.Token);

        if (abuseCheck.AbuseConfidenceScore > 70)
        {
            var error = new ErrorModel
            {
                Title = "Bad Request.",
                Detail = "User IP is blocked due to abuse.",
                Instance = $"/action/{Activity.Current?.Id ?? httpContextAccessor.HttpContext.TraceIdentifier}",
                Status = 400,
                Type = "/action/abusiveip"
            };

            // return a 400 with the error information
            return Unauthorized(error);
        }
    }
}
catch (OperationCanceledException)
{
    // request timed out.
}
catch (ApiException abuseIPException)
{
    // abuse ip db threw an exception
}
```