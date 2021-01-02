namespace Kenc.AbuseIPDB.Exceptions
{
    using System;
    using System.Net;
    using Kenc.AbuseIPDB.ApiReplies;

    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public Error[] Errors { get; set; }

        public RateLimit RateLimit { get; set; }

        public ApiException(HttpStatusCode statusCode, Error[] errors, RateLimit rateLimit) : base($"AbuseIPDB returned errors. See {nameof(Errors)} for details.")
        {
            Errors = errors;
            StatusCode = statusCode;
            RateLimit = rateLimit;
        }
    }
}
