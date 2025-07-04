﻿using System;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Represents a product header value. This is used to generate the User Agent string sent with each request. The
    /// name used should represent the product, the GitHub Organization, or the GitHub username that's using DynamicWebTWAIN.RestClient.net (in that order of preference).
    /// </summary>
    /// <remarks>
    /// This class is a wrapper around <seealso href="https://msdn.microsoft.com/en-us/library/system.net.http.headers.productheadervalue(v=vs.118).aspx"/>
    /// so that consumers of DynamicWebTWAIN.RestClient.net would not have to add a reference to the System.Net.Http.Headers namespace.
    /// See more information regarding User-Agent requirements here: https://developer.github.com/v3/#user-agent-required
    /// </remarks>
    public class ProductHeaderValue
    {
        readonly System.Net.Http.Headers.ProductHeaderValue _productHeaderValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductHeaderValue"/> class.
        /// </summary>
        /// <remarks>
        /// See more information regarding User-Agent requirements here: https://developer.github.com/v3/#user-agent-required
        /// </remarks>
        /// <param name="name">The name of the product, the GitHub Organization, or the GitHub Username (in that order of preference) that's using DynamicWebTWAIN.RestClient</param>
        public ProductHeaderValue(string name)
            : this(new System.Net.Http.Headers.ProductHeaderValue(name))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductHeaderValue"/> class.
        /// </summary>
        /// <remarks>
        /// See more information regarding User-Agent requirements here: https://developer.github.com/v3/#user-agent-required
        /// </remarks>
        /// <param name="name">The name of the product, the GitHub Organization, or the GitHub Username (in that order of preference) that's using DynamicWebTWAIN.RestClient</param>
        /// <param name="version">The version of the product that's using DynamicWebTWAIN.RestClient</param>
        public ProductHeaderValue(string name, string version)
            : this(new System.Net.Http.Headers.ProductHeaderValue(name, version))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductHeaderValue"/> class.
        /// </summary>
        /// <remarks>
        /// See more information regarding User-Agent requirements here: https://developer.github.com/v3/#user-agent-required
        /// </remarks>
        /// <param name="productHeader">The <see cref="System.Net.Http.Headers.ProductHeaderValue"/>.</param>
        public ProductHeaderValue(System.Net.Http.Headers.ProductHeaderValue productHeader)
        {
            _productHeaderValue = productHeader ?? throw new ArgumentNullException(nameof(productHeader));
        }

        /// <summary>
        /// The name of the product, the GitHub Organization, or the GitHub Username that's using DynamicWebTWAIN.RestClient (in that order of preference)
        /// </summary>
        /// <remarks>
        /// See more information regarding User-Agent requirements here: https://developer.github.com/v3/#user-agent-required
        /// </remarks>
        public string Name
        {
            get { return _productHeaderValue.Name; }
        }

        /// <summary>
        /// The version of the product.
        /// </summary>
        public string Version
        {
            get { return _productHeaderValue.Version; }
        }

        public override bool Equals(object obj)
        {
            return _productHeaderValue.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _productHeaderValue.GetHashCode();
        }

        public override string ToString()
        {
            return _productHeaderValue.ToString();
        }

        /// <summary>
        /// Parses a string in the format "foo" or "foo/1.0" and returns the corresponding
        /// <see cref="ProductHeaderValue" /> instance.
        /// </summary>
        /// <param name="input">The input.</param>
        public static ProductHeaderValue Parse(string input)
        {
            return new ProductHeaderValue(System.Net.Http.Headers.ProductHeaderValue.Parse(input));
        }

        /// <summary>
        /// Parses a string in the format "foo" or "foo/1.0" and returns the corresponding
        /// <see cref="ProductHeaderValue" /> instance via an out parameter.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="parsedValue">The parsed value.</param>
        public static bool TryParse(string input,
            out ProductHeaderValue parsedValue)
        {
            System.Net.Http.Headers.ProductHeaderValue value;
            var result = System.Net.Http.Headers.ProductHeaderValue.TryParse(input, out value);
            parsedValue = result ? Parse(input) : null;
            return result;
        }
    }
}
