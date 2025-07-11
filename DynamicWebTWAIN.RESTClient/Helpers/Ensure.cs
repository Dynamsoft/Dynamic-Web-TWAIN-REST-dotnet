﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    ///   Ensure input parameters
    /// </summary>
    public static class Ensure
    {
        /// <summary>
        /// Checks an argument to ensure it isn't null.
        /// </summary>
        /// <param name = "value">The argument value to check</param>
        /// <param name = "name">The name of the argument</param>
        public static void ArgumentNotNull([ValidatedNotNull]object value, string name)
        {
            if (value != null) return;

            throw new ArgumentNullException(name);
        }
        
        /// <summary>
        /// Checks an argument to ensure it isn't null or the default value.
        /// </summary>
        /// <param name = "value">The argument value to check</param>
        /// <param name = "name">The name of the argument</param>
        public static void ArgumentNotNullOrDefault<T>([ValidatedNotNull]T value, string name)
        {
            if (value != null && !EqualityComparer<T>.Default.Equals(value, default)) return;

            throw new ArgumentNullException(name);
        }

        /// <summary>
        /// Checks a string argument to ensure it isn't null or empty.
        /// </summary>
        /// <param name = "value">The argument value to check</param>
        /// <param name = "name">The name of the argument</param>
        public static void ArgumentNotNullOrEmptyString([ValidatedNotNull]string value, string name)
        {
            ArgumentNotNull(value, name);
            if (!string.IsNullOrWhiteSpace(value)) return;

            throw new ArgumentException("String cannot be empty", name);
        }

        /// <summary>
        /// Checks a timespan argument to ensure it is a positive value.
        /// </summary>
        /// <param name = "value">The argument value to check</param>
        /// <param name = "name">The name of the argument</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void GreaterThanZero([ValidatedNotNull]TimeSpan value, string name)
        {
            ArgumentNotNull(value, name);

            if (value.TotalMilliseconds > 0) return;

            throw new ArgumentException("Timespan must be greater than zero", name);
        }

        /// <summary>
        /// Checks an integer argument to ensure it is a positive value.
        /// </summary>
        /// <param name = "value">The argument value to check</param>
        /// <param name = "name">The name of the argument</param>
        public static void GreaterThanZero([ValidatedNotNull] int value, string name)
        {
            ArgumentNotNull(value, name);

            if (value > 0) return;

            throw new ArgumentException("Value must be greater than zero", name);
        }

        /// <summary>
        /// Checks an enumerable argument to ensure it isn't null or empty.
        /// </summary>
        /// <param name = "value">The argument value to check</param>
        /// <param name = "name">The name of the argument</param>
        public static void ArgumentNotNullOrEmptyEnumerable<T>([ValidatedNotNull]IEnumerable<T> value, string name)
        {
            ArgumentNotNull(value, name);
            if (Enumerable.Any(value)) return;

            throw new ArgumentException("List cannot be empty", name);
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class ValidatedNotNullAttribute : Attribute
    {
    }
}
