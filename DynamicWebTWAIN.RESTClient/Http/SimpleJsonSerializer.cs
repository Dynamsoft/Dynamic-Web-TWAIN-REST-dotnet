﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using DynamicWebTWAIN.RestClient.Reflection;

namespace DynamicWebTWAIN.RestClient.Internal
{
    public class SimpleJsonSerializer : IJsonSerializer
    {
        static readonly GitHubSerializerStrategy _serializationStrategy = new GitHubSerializerStrategy();

        /// <summary>
        /// Serializes the given object to a JSON string.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public string Serialize(object item)
        {
            return SimpleJson.SerializeObject(item, _serializationStrategy);
        }

        /// <summary>
        /// Deserializes the given JSON string to an object of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public T Deserialize<T>(string json)
        {
            return SimpleJson.DeserializeObject<T>(json, _serializationStrategy);
        }

        /// <summary>
        /// Serializes the given enum value to a JSON string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string SerializeEnum(Enum value)
        {
            return _serializationStrategy.SerializeEnumHelper(value).ToString();
        }

        /// <summary>
        /// Deserializes the given JSON string to an enum value of the specified type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static object DeserializeEnum(string value, Type type)
        {
            return _serializationStrategy.DeserializeEnumHelper(value, type);
        }

        /// <summary>
        /// This class is responsible for serializing and deserializing objects to and from JSON.
        /// </summary>
        class GitHubSerializerStrategy : PocoJsonSerializerStrategy
        {
            readonly List<string> _membersWhichShouldPublishNull = new List<string>();
            readonly ConcurrentDictionary<Type, ConcurrentDictionary<object, object>> _cachedEnums = new ConcurrentDictionary<Type, ConcurrentDictionary<object, object>>();

            protected override string MapClrMemberToJsonFieldName(MemberInfo member)
            {
                return member.GetJsonFieldName();
            }

            internal override IDictionary<string, ReflectionUtils.GetDelegate> GetterValueFactory(Type type)
            {
                var propertiesAndFields = type.GetPropertiesAndFields().Where(p => p.CanSerialize).ToList();

                foreach (var property in propertiesAndFields.Where(p => p.SerializeNull))
                {
                    var key = type.FullName + "-" + property.JsonFieldName;

                    _membersWhichShouldPublishNull.Add(key);
                }

                return propertiesAndFields
                    .ToDictionary(
                        p => p.JsonFieldName,
                        p => p.GetDelegate);
            }

            // This is overridden so that null values are omitted from serialized objects.
            [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "Need to support .NET 2")]
            protected override bool TrySerializeUnknownTypes(object input, out object output)
            {
                Ensure.ArgumentNotNull(input, nameof(input));

                var type = input.GetType();
                var getters = GetCache[type];

                if (ReflectionUtils.IsStringEnumWrapper(type))
                {
                    // Handle StringEnum<T> by getting the underlying enum value, then using the enum serializer
                    // Note this will throw if the StringEnum<T> was initialized using a string that is not a valid enum member
                    var inputEnum = (getters["value"](input) as Enum);
                    if (inputEnum != null)
                    {
                        output = SerializeEnum(inputEnum);
                        return true;
                    }
                }

                var jsonObject = new JsonObject();
                foreach (var getter in getters)
                {
                    if (getter.Value != null)
                    {
                        var value = getter.Value(input);
                        if (value == null)
                        {
                            var key = type.FullName + "-" + getter.Key;
                            if (!_membersWhichShouldPublishNull.Contains(key))
                                continue;
                        }
                        jsonObject.Add(getter.Key, value);
                    }
                }
                output = jsonObject;
                return true;
            }

            internal object SerializeEnumHelper(Enum p)
            {
                return SerializeEnum(p);
            }

            [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",
                Justification = "The API expects lowercase values")]
            protected override object SerializeEnum(Enum p)
            {
                return p.ToParameter();
            }

            internal object DeserializeEnumHelper(string value, Type type)
            {
                var cachedEnumsForType = _cachedEnums.GetOrAdd(type, t =>
                {
                    var enumsForType = new ConcurrentDictionary<object, object>();

                    // Try to get all custom attributes, this happens only once per type
                    var fields = type.GetRuntimeFields();
                    foreach (var field in fields)
                    {
                        if (field.Name == "value__")
                            continue;
                        var attribute = (ParameterAttribute)field.GetCustomAttribute(typeof(ParameterAttribute));
                        if (attribute != null)
                        {
                            enumsForType.GetOrAdd(attribute.Value, _ => field.GetValue(null));
                        }
                    }

                    return enumsForType;
                });

                // If type cache does not contain enum value and has no custom attribute, add it for future loops
                return cachedEnumsForType.GetOrAdd(value, v => Enum.Parse(type, value, ignoreCase: true));
            }

            // Overridden to handle enums.
            public override object DeserializeObject(object value, Type type)
            {
                var stringValue = value as string;
                var jsonValue = value as JsonObject;

                if (stringValue != null)
                {
                    // If it's a nullable type, use the underlying type
                    if (ReflectionUtils.IsNullableType(type))
                    {
                        type = Nullable.GetUnderlyingType(type);
                    }

                    var typeInfo = ReflectionUtils.GetTypeInfo(type);

                    if (typeInfo.IsEnum)
                    {
                        return DeserializeEnumHelper(stringValue, type);
                    }

                    if (ReflectionUtils.IsTypeGenericeCollectionInterface(type))
                    {
                        // OAuth tokens might be a string of comma-separated values
                        // we should only try this if the return array is a collection of strings
                        var innerType = ReflectionUtils.GetGenericListElementType(type);
                        if (innerType.IsAssignableFrom(typeof(string)))
                        {
                            return stringValue.Split(',');
                        }
                    }

                    if (ReflectionUtils.IsStringEnumWrapper(type))
                    {
                        return Activator.CreateInstance(type, stringValue);
                    }
                }

                if (ReflectionUtils.IsStringEnumWrapper(type))
                {
                    // this check is a workaround for https://github.com/octokit/octokit.net/issues/2052
                    // as the API is returning a null value where the enum is
                    // expecting something like a string
                    //
                    // this should be removed once we can confirm the GitHub API
                    // is no longer returning a null for the parent Team's
                    // permission value
                    return Activator.CreateInstance(type, "null");
                }

                return base.DeserializeObject(value, type);
            }

            /// <summary>
            /// This method is used to create a dictionary of properties and fields that can be set.
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            internal override IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> SetterValueFactory(Type type)
            {
                return type.GetPropertiesAndFields()
                    .Where(p => p.CanDeserialize)
                    .ToDictionary(
                        p => p.JsonFieldName,
                        p => new KeyValuePair<Type, ReflectionUtils.SetDelegate>(p.Type, p.SetDelegate));
            }
        }
    }
}
