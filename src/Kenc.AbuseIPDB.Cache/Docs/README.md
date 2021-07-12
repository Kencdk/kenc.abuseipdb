# About #
Kenc.AbuseIPDB.Cache adds a memory cache over the AbuseIPDB client.

# How to use #

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