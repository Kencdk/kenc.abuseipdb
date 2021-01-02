namespace Kenc.AbuseIPDB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Headers;

    public class RateLimit
    {
        private const string RetryAfterHeader = "Retry-After";
        private const string XRateLimitResetHeader = "X-RateLimit-Reset";
        private const string XRateLimitLimitHeader = "X-RateLimit-Limit";
        private const string XRateLimitRemainingHeader = "X-RateLimit-Remaining";

        public uint RetryAfter { get; set; }

        public uint Limit { get; set; }

        public uint Remaining { get; set; }

        public DateTimeOffset? Reset { get; set; }

        public RateLimit(uint retryAfter, uint limit, uint remaining, DateTimeOffset? reset)
        {
            Reset = reset;
            Limit = limit;
            Remaining = remaining;
            RetryAfter = retryAfter;
        }

        internal static RateLimit FromHttpResponseHeaders(HttpResponseHeaders headers)
        {
            // retryafter is only sent it we are throttled.
            uint retryAfterValue = 0;
            if (headers.TryGetValues(RetryAfterHeader, out IEnumerable<string> retryAfterValues))
            {
                retryAfterValue = Convert.ToUInt32(retryAfterValues.FirstOrDefault());
            }

            // reset is only sent it we are throttled.
            DateTimeOffset? resetTime = null;
            if (headers.TryGetValues(XRateLimitResetHeader, out IEnumerable<string> resetValues))
            {
                var resetValue = Convert.ToUInt32(resetValues.FirstOrDefault());
                resetTime = DateTimeOffset.FromUnixTimeSeconds(resetValue);
            }

            uint limitValue = 0;
            if (headers.TryGetValues(XRateLimitLimitHeader, out IEnumerable<string> limitValues))
            {
                limitValue = Convert.ToUInt32(limitValues.FirstOrDefault());
            }

            uint remainingValue = 0;
            if (headers.TryGetValues(XRateLimitRemainingHeader, out IEnumerable<string> remainingValues))
            {
                remainingValue = Convert.ToUInt32(remainingValues.FirstOrDefault());
            }

            return new RateLimit(retryAfterValue, limitValue, remainingValue, resetTime);
        }
    }
}
