using System;
using System.Net;

using ApplicationException = System.InvalidOperationException;

namespace JagiCore.Mvc
{
    public class HttpResponseException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseException"/> class.
        /// </summary>
        /// <param name="httpStatusCode"><see cref="HttpStatusCode"/> value.</param>
        public HttpResponseException(HttpStatusCode httpStatusCode)
            : base()
        {
            this.HttpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseException"/> class.
        /// </summary>
        /// <param name="httpStatusCode"><see cref="HttpStatusCode"/> value.</param>
        /// <param name="message">Exception message.</param>
        public HttpResponseException(HttpStatusCode httpStatusCode, string message)
            : base(message)
        {
            this.HttpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseException"/> class.
        /// </summary>
        /// <param name="httpStatusCode"><see cref="HttpStatusCode"/> value.</param>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner <see cref="Exception"/> object.</param>
        public HttpResponseException(HttpStatusCode httpStatusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            this.HttpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Gets or sets the <see cref="HttpStatusCode"/> value.
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}