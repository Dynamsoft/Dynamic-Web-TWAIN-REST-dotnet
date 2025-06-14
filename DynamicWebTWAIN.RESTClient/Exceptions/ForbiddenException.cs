﻿// Copyright (c) 2017 GitHub, Inc.
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.Serialization;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Represents a HTTP 403 - Forbidden response returned from the API.
    /// </summary>
    [Serializable]
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors",
        Justification = "These exceptions are specific to the GitHub API and not general purpose exceptions")]
    public class ForbiddenException : ApiException
    {
        /// <summary>
        /// Constructs an instance of ForbiddenException
        /// </summary>
        /// <param name="response">The HTTP payload from the server</param>
        public ForbiddenException(IResponse response) : this(response, null)
        {
        }

        /// <summary>
        /// Constructs an instance of ForbiddenException
        /// </summary>
        /// <param name="response">The HTTP payload from the server</param>
        /// <param name="innerException">The inner exception</param>
        public ForbiddenException(IResponse response, Exception innerException)
            : base(response, innerException)
        {
            Debug.Assert(response != null && response.StatusCode == HttpStatusCode.Forbidden,
                "ForbiddenException created with wrong status code");
        }

        /// <summary>
        /// Constructs an instance of ForbiddenException
        /// </summary>
        public override string Message
        {
            get { return ApiErrorMessageSafe ?? "Request Forbidden"; }
        }

        /// <summary>
        /// Constructs an instance of ForbiddenException
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the
        /// serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains
        /// contextual information about the source or destination.
        /// </param>
        protected ForbiddenException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
