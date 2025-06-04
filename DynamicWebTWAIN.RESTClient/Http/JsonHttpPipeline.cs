using System;
using System.Collections;
using System.IO;
using System.Net.Http;

namespace DynamicWebTWAIN.RestClient.Internal
{
    /// <summary>
    ///     Responsible for serializing the request and response as JSON and
    ///     adding the proper JSON response header.
    /// </summary>
    public class JsonHttpPipeline
    {
        readonly IJsonSerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonHttpPipeline" /> class.
        /// </summary>
        public JsonHttpPipeline() : this(new SimpleJsonSerializer())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonHttpPipeline" /> class.
        /// </summary>
        /// <param name="serializer"></param>
        public JsonHttpPipeline(IJsonSerializer serializer)
        {
            Ensure.ArgumentNotNull(serializer, nameof(serializer));

            _serializer = serializer;
        }

        /// <summary>
        /// Serializes the object to a JSON string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Serialize(object value)
        {
            return _serializer.Serialize(value);
        }


        /// <summary>
        /// Serializes the request body to a JSON string if it is not already a string or stream.
        /// </summary>
        /// <param name="request"></param>
        public void SerializeRequest(IRequest request)
        {
            Ensure.ArgumentNotNull(request, nameof(request));

            if (request.Method == HttpMethod.Get || request.Body == null) return;
            if (request.Body is string || request.Body is Stream || request.Body is HttpContent) return;

            request.Body = _serializer.Serialize(request.Body);
        }

        /// <summary>
        /// Deserializes the response body from a JSON string to the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        public IApiResponse<T> DeserializeResponse<T>(IResponse response)
        {
            Ensure.ArgumentNotNull(response, nameof(response));

            if (response.ContentType != null && response.ContentType.Equals("application/json", StringComparison.Ordinal))
            {
                var body = response.Body as string;
                // simple json does not support the root node being empty. Will submit a pr but in the mean time....
                if (!string.IsNullOrEmpty(body) && body != "{}")
                {
                    var typeIsDictionary = typeof(IDictionary).IsAssignableFrom(typeof(T)) || typeof(T).IsAssignableToGenericType(typeof(System.Collections.Generic.IDictionary<,>));
                    var typeIsEnumerable = typeof(IEnumerable).IsAssignableFrom(typeof(T));
                    var responseIsObject = body.StartsWith("{", StringComparison.Ordinal);

                    // If we're expecting an array, but we get a single object, just wrap it.
                    // This supports an API that dynamically changes the return type based on the content.
                    if (!typeIsDictionary && typeIsEnumerable && responseIsObject)
                    {
                        body = "[" + body + "]";
                    }
                    var json = _serializer.Deserialize<T>(body);
                    return new ApiResponse<T>(response, json);
                }
            }
            return new ApiResponse<T>(response);
        }
    }
}
