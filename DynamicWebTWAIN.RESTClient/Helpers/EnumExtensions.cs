using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using DynamicWebTWAIN.RestClient.Internal;

namespace DynamicWebTWAIN.RestClient
{
    static class EnumExtensions
    {
        /// <summary>
        /// Converts an enum to a parameter string.
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        public static string ToParameter(this Enum prop)
        {
            if (prop == null) return null;

            var propString = prop.ToString();
            var member = prop.GetType().GetMember(propString).FirstOrDefault();

            if (member == null) return null;

            var attribute = member.GetCustomAttributes(typeof(ParameterAttribute), false)
                .Cast<ParameterAttribute>()
                .FirstOrDefault();

            return attribute != null ? attribute.Value : propString.ToLowerInvariant();
        }

        /// <summary>
        /// Checks if the enum has a parameter attribute.
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        internal static bool HasParameter(this Enum prop)
        {
            if (prop == null) return false;

            var propString = prop.ToString();
            var member = prop.GetType().GetMember(propString).FirstOrDefault();

            if (member == null) return false;

            var attribute = member.GetCustomAttributes(typeof(ParameterAttribute), false)
                .Cast<ParameterAttribute>()
                .FirstOrDefault();

            return attribute != null;
        }
    }
}
