namespace Kenc.AbuseIPDB.Exceptions
{
    using System;
    using System.Net;
    using Kenc.AbuseIPDB.ApiReplies;

    /// <summary>
    /// Exception class wrapping errors returned from AbuseIPDB API.
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// Gets the status code returned in the HTTP response.
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Gets the errors returned in the response body.
        /// </summary>
        public Error[] Errors { get; private set; }

        /// <summary>
        /// Gets the rate limit header.
        /// </summary>
        public RateLimit RateLimit { get; private set; }

        public ApiException(HttpStatusCode statusCode, Error[] errors, RateLimit rateLimit) : base($"AbuseIPDB returned errors. See {nameof(Errors)} for details.")
        {
            Errors = errors;
            StatusCode = statusCode;
            RateLimit = rateLimit;
        }
    }
}
