using System;
using System.Collections.Generic;

namespace DynamicWebTWAIN.RestClient
{
    public static class ParameterBuilder
    {
        /// <summary>
        /// Creates a new dictionary with a single key-value pair.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Dictionary<string, string> AddParameter(string key, string value)
        {
            Ensure.ArgumentNotNullOrEmptyString(key, nameof(key));
            Ensure.ArgumentNotNullOrEmptyString(value, nameof(value));

            return new Dictionary<string, string> { { key, value } };
        }

        /// <summary>
        /// Creates a new dictionary with a single key-value pair.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Dictionary<string, string> AddParameter(string key, Enum value)
        {
            Ensure.ArgumentNotNullOrEmptyString(key, nameof(key));
            Ensure.ArgumentNotNull(value, nameof(value));

            if (value.HasParameter())
            {
                return new Dictionary<string, string> { { key, value.ToParameter() } };
            }

            return new Dictionary<string, string> { { key, value.ToString() } };

        }

        /// <summary>
        /// Adds a key-value pair to the dictionary.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Dictionary<string, string> AddParameter(this Dictionary<string, string> data, string key, string value)
        {
            Ensure.ArgumentNotNull(data, nameof(data));
            Ensure.ArgumentNotNullOrEmptyString(key, nameof(key));
            Ensure.ArgumentNotNullOrEmptyString(value, nameof(value));

            data.Add(key, value);
            return data;
        }

        /// <summary>
        /// Adds a key-value pair to the dictionary.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Dictionary<string, string> AddParameter(this Dictionary<string, string> data, string key, Enum value)
        {
            Ensure.ArgumentNotNull(data, nameof(data));
            Ensure.ArgumentNotNullOrEmptyString(key, nameof(key));
            Ensure.ArgumentNotNull(value, nameof(value));

            if (value.HasParameter())
            {
                data.Add(key, value.ToParameter());
            }
            else
            {
                data.Add(key, value.ToString());
            }
            
            return data;
        }

        /// <summary>
        /// Adds an optional key-value pair to the dictionary.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Dictionary<string, string> AddOptionalParameter(this Dictionary<string, string> data, string key, string value)
        {
            Ensure.ArgumentNotNull(data, nameof(data));
            Ensure.ArgumentNotNullOrEmptyString(key, nameof(key));

            if (value != null)
            {
                data.Add(key, value);
            }
            return data;
        }

        /// <summary>
        /// Adds an optional key-value pair to the dictionary.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Dictionary<string, string> AddOptionalParameter(this Dictionary<string, string> data, string key, Enum value)
        {
            Ensure.ArgumentNotNull(data, nameof(data));
            Ensure.ArgumentNotNullOrEmptyString(key, nameof(key));

            if (value != null)
            {
                if (value.HasParameter())
                {
                    data.Add(key, value.ToParameter());
                }
                else
                {
                    data.Add(key, value.ToString());
                }
            }

            return data;
        }
    }
}
