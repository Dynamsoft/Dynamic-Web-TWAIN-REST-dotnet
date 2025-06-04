using DynamicWebTWAIN.RestClient.Internal;
using DynamicWebTWAIN.RestClient.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DynamicWebTWAIN.RestClient
{
    internal static class ReflectionExtensions
    {
        /// <summary>
        /// Gets the JSON field name for a member.
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static string GetJsonFieldName(this MemberInfo memberInfo)
        {
            var memberName = memberInfo.Name;
            var paramAttr = memberInfo.GetCustomAttribute<ParameterAttribute>();

            if (paramAttr != null && !string.IsNullOrEmpty(paramAttr.Key))
            {
                // use the parameter key directly
                return paramAttr.Key;
            }

            return memberName.ToRubyCase();
        }

        /// <summary>
        /// Gets the JSON field name for a member.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyOrField> GetPropertiesAndFields(this Type type)
        {
            return ReflectionUtils.GetProperties(type).Select(property => new PropertyOrField(property))
                .Union(ReflectionUtils.GetFields(type).Select(field => new PropertyOrField(field)))
                .Where(p => (p.IsPublic || p.HasParameterAttribute) && !p.IsStatic);
        }

        /// <summary>
        /// Checks if the type is a DateTimeOffset.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDateTimeOffset(this Type type)
        {
            return type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?);
        }

        /// <summary>
        /// Checks if the type is a nullable type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullable(this Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Gets the member info for a given type and name.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnumerable<MemberInfo> GetMember(this Type type, string name)
        {
            return type.GetTypeInfo().DeclaredMembers.Where(m => m.Name == name);
        }

        /// <summary>
        /// Gets the property info for a given type and property name.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static PropertyInfo GetProperty(this Type t, string propertyName)
        {
            return t.GetTypeInfo().GetDeclaredProperty(propertyName);
        }

        /// <summary>
        /// Checks if the type is assignable from another type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public static bool IsAssignableFrom(this Type type, Type otherType)
        {
            return type.GetTypeInfo().IsAssignableFrom(otherType.GetTypeInfo());
        }

        /// <summary>
        /// Gets all properties of a type, including inherited properties.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            var properties = typeInfo.DeclaredProperties;

            var baseType = typeInfo.BaseType;

            return baseType == null ? properties : properties.Concat(baseType.GetAllProperties());
        }

        /// <summary>
        /// Checks if the type is an enumeration.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsEnumeration(this Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }
    }
}
