﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DynamicWebTWAIN.RestClient.Internal;

namespace DynamicWebTWAIN.RestClient
{
    /// <summary>
    /// Base class for classes which represent query string parameters to certain API endpoints.
    /// </summary>
    public abstract class RequestParameters
    {
        static readonly ConcurrentDictionary<Type, List<PropertyParameter>> _propertiesMap =
            new ConcurrentDictionary<Type, List<PropertyParameter>>();

        /// <summary>
        /// Converts the derived object into a dictionary that can be used to supply query string parameters.
        /// </summary>
        /// <returns></returns>
        public virtual IDictionary<string, string> ToParametersDictionary()
        {
            var map = _propertiesMap.GetOrAdd(GetType(), GetPropertyParametersForType);
            return (from property in map
                    let value = property.GetValue(this)
                    let key = property.Key
                    where value != null
                    select new { key, value }).ToDictionary(kvp => kvp.key, kvp => kvp.value);
        }

        /// <summary>
        /// Gets the list of properties for the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static List<PropertyParameter> GetPropertyParametersForType(Type type)
        {
            return type.GetAllProperties()
                .Where(p => p.Name != "DebuggerDisplay")
                .Select(p => new PropertyParameter(p))
                .ToList();
        }

        /// <summary>
        /// Gets the value function for the given property type.
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",
            Justification = "GitHub API depends on lower case strings")]
        static Func<PropertyInfo, object, string> GetValueFunc(Type propertyType)
        {
            // get underlying type if nullable
            propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            if (typeof(IEnumerable<string>).IsAssignableFrom(propertyType))
            {
                return (prop, value) =>
                {
                    var list = ((IEnumerable<string>)value).ToArray();
                    return !list.Any() ? null : string.Join(",", list);
                };
            }

            if (propertyType.IsDateTimeOffset())
            {
                return (prop, value) =>
                {
                    if (value == null) return null;
                    return ((DateTimeOffset)value).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ",
                      CultureInfo.InvariantCulture);
                };
            }

            if (propertyType.IsEnumeration())
            {
                var enumToAttributeDictionary = Enum.GetNames(propertyType)
                    .ToDictionary(name => name, name => GetParameterAttributeValueForEnumName(propertyType, name));
                return (prop, value) =>
                {
                    if (value == null) return null;
                    string attributeValue;

                    return enumToAttributeDictionary.TryGetValue(value.ToString(), out attributeValue)
                        ? attributeValue ?? value.ToString().ToLowerInvariant()
                        : value.ToString().ToLowerInvariant();
                };
            }

            if (typeof(bool).IsAssignableFrom(propertyType))
            {
                // GitHub does not recognize title-case boolean values as arguments.
                // We need to convert them to lowercase.
                return (prop, value) => value != null ? value.ToString().ToLowerInvariant() : null;
            }

            return (prop, value) => value != null
                ? value.ToString()
                : null;
        }

        /// <summary>
        /// Gets the parameter attribute value for the enum name.
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        static string GetParameterAttributeValueForEnumName(Type enumType, string name)
        {
            var member = enumType.GetMember(name).FirstOrDefault();

            if (member == null) return null;
            var attribute = member.GetCustomAttributes(typeof(ParameterAttribute), false)
                .Cast<ParameterAttribute>()
                .FirstOrDefault();
            return attribute != null ? attribute.Value : null;
        }

        /// <summary>
        /// Represents a property parameter.
        /// </summary>
        class PropertyParameter
        {
            readonly Func<PropertyInfo, object, string> _valueFunc;
            readonly PropertyInfo _property;
            public PropertyParameter(PropertyInfo property)
            {
                _property = property;
                Key = GetParameterKeyFromProperty(property);
                _valueFunc = GetValueFunc(property.PropertyType);
            }

            public string Key { get; private set; }

            public string GetValue(object instance)
            {
                return _valueFunc(_property, _property.GetValue(instance, null));
            }

            [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",
                Justification = "GitHub API depends on lower case strings")]
            static string GetParameterKeyFromProperty(PropertyInfo property)
            {
                var attribute = property.GetCustomAttributes(typeof(ParameterAttribute), false)
                    .Cast<ParameterAttribute>()
                    .FirstOrDefault(attr => attr.Key != null);

                return attribute == null
                    ? property.Name.ToLowerInvariant()
                    : attribute.Key;
            }
        }
    }
}
