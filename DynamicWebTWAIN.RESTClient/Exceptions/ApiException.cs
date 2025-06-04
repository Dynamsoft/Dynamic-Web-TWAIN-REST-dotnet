// Copyright (c) 2017 GitHub, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of 
// this software and associated documentation files (the "Software"), to deal in 
// the Software without restriction, including without limitation the rights to 
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER 
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// original code from https://github.com/octokit/octokit.net

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.Serialization;
using System.Security;
using DynamicWebTWAIN.RestClient.Internal;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Represents errors that occur from the GitHub API.
    /// </summary>
    [Serializable]
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors",
        Justification = "These exceptions are specific to the DWT API and not general purpose exceptions")]
    public class ApiException : Exception
    {
        // This needs to be hard-coded for translating GitHub error messages.
        static readonly IJsonSerializer _jsonSerializer = new SimpleJsonSerializer();

        /// <summary>
        /// Constructs an instance of ApiException
        /// </summary>
#pragma warning disable CS0618 // Response() is obsolete but we need this as a default as Response passed down cannot be null
        public ApiException() : this(new Response())
#pragma warning restore CS0618 // Response() is obsolete but we need this as a default as Response passed down cannot be null
        {
        }

        /// <summary>
        /// Constructs an instance of ApiException
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="httpStatusCode">The HTTP status code from the response</param>
        public ApiException(string message, HttpStatusCode httpStatusCode)
            : this(GetApiErrorFromExceptionMessage(message), httpStatusCode, null)
        {
        }

        /// <summary>
        /// Constructs an instance of ApiException
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="innerException">The inner exception</param>
        public ApiException(string message, Exception innerException)
            : this(GetApiErrorFromExceptionMessage(message), 0, innerException)
        {
        }

        /// <summary>
        /// Constructs an instance of ApiException
        /// </summary>
        /// <param name="response">The HTTP payload from the server</param>
        public ApiException(IResponse response)
            : this(response, null)
        {
        }

        /// <summary>
        /// Constructs an instance of ApiException
        /// </summary>
        /// <param name="response">The HTTP payload from the server</param>
        /// <param name="innerException">The inner exception</param>
        public ApiException(IResponse response, Exception innerException)
            : base(null, innerException)
        {
            Ensure.ArgumentNotNull(response, nameof(response));

            StatusCode = response.StatusCode;
            ApiError = GetApiErrorFromExceptionMessage(response);
            HttpResponse = response;
        }

        /// <summary>
        /// Constructs an instance of ApiException
        /// </summary>
        /// <param name="innerException">The inner exception</param>
        protected ApiException(ApiException innerException)
        {
            Ensure.ArgumentNotNull(innerException, nameof(innerException));

            StatusCode = innerException.StatusCode;
            ApiError = innerException.ApiError;
        }

        /// <summary>
        /// Constructs an instance of ApiException
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="innerException"></param>
        protected ApiException(HttpStatusCode statusCode, Exception innerException)
            : base(null, innerException)
        {
            ApiError = new ApiError();
            StatusCode = statusCode;
        }

        /// <summary>
        /// Constructs an instance of ApiException
        /// </summary>
        /// <param name="apiError"></param>
        /// <param name="statusCode"></param>
        /// <param name="innerException"></param>
        protected ApiException(ApiError apiError, HttpStatusCode statusCode, Exception innerException)
            : base(null, innerException)
        {
            Ensure.ArgumentNotNull(apiError, nameof(apiError));

            ApiError = apiError;
            StatusCode = statusCode;
        }


        /// <summary>
        /// The HTTP payload from the server
        /// </summary>
        public IResponse HttpResponse { get; private set; }

        public override string Message
        {
            get { return ApiErrorMessageSafe ?? "An error occurred with this API request"; }
        }

        /// <summary>
        /// The HTTP status code associated with the response
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// The raw exception payload from the response
        /// </summary>
        public ApiError ApiError { get; private set; }

        /// <summary>
        /// Constructs an instance of ApiError from the response
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        static ApiError GetApiErrorFromExceptionMessage(IResponse response)
        {
            string responseBody = response != null ? response.Body as string : null;
            return GetApiErrorFromExceptionMessage(responseBody);
        }

        /// <summary>
        /// Constructs an instance of ApiError from the response
        /// </summary>
        /// <param name="responseContent"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        static ApiError GetApiErrorFromExceptionMessage(string responseContent)
        {
            try
            {
                if (!string.IsNullOrEmpty(responseContent))
                {
                    return _jsonSerializer.Deserialize<ApiError>(responseContent) ?? new ApiError(responseContent);
                }
            }
            catch (Exception)
            {
            }

            return new ApiError(responseContent);
        }

        /// <summary>
        /// Constructs an instance of ApiException.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the
        /// serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains
        /// contextual information about the source or destination.
        /// </param>
        protected ApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null) return;
            StatusCode = (HttpStatusCode)info.GetInt32("HttpStatusCode");
            ApiError = (ApiError)info.GetValue("ApiError", typeof(ApiError));
        }

        /// <summary>
        /// Serializes the exception data to the <see cref="SerializationInfo"/> object.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("HttpStatusCode", StatusCode);
            info.AddValue("ApiError", ApiError);
        }

        /// <summary>
        /// Get the inner error message from the API response
        /// </summary>
        /// <remarks>
        /// Returns null if ApiError is not populated
        /// </remarks>
        protected string ApiErrorMessageSafe
        {
            get
            {
                if (ApiError != null && !string.IsNullOrWhiteSpace(ApiError.Message))
                {
                    return ApiError.Message;
                }

                return null;
            }
        }

        /// <summary>
        /// Get the inner http response body from the API response
        /// </summary>
        /// <remarks>
        /// Returns empty string if HttpResponse is not populated or if
        /// response body is not a string
        /// </remarks>
        protected string HttpResponseBodySafe
        {
            get
            {
                return HttpResponse?.ContentType != null
                       && !HttpResponse.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
                       && HttpResponse.Body is string @string
                       ? @string : string.Empty;
            }
        }

        /// <summary>
        /// Returns a string representation of the exception
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var original = base.ToString();
            return original + Environment.NewLine + HttpResponseBodySafe;
        }
    }
}
