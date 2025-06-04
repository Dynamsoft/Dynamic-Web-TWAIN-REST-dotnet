using System;
using System.Reflection;
using DynamicWebTWAIN.RestClient.Helpers;
using DynamicWebTWAIN.RestClient.Internal;
using DynamicWebTWAIN.RestClient.Reflection;

namespace DynamicWebTWAIN.RestClient
{
    internal class PropertyOrField
    {
        readonly PropertyInfo _propertyInfo;
        readonly FieldInfo _fieldInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyOrField"/> class.
        /// </summary>
        /// <param name="propertyInfo"></param>
        public PropertyOrField(PropertyInfo propertyInfo) : this((MemberInfo)propertyInfo)
        {
            _propertyInfo = propertyInfo;

            CanRead = propertyInfo.CanRead;
            CanWrite = propertyInfo.CanWrite;
            IsStatic = ReflectionUtils.GetGetterMethodInfo(propertyInfo).IsStatic;
            IsPublic = ReflectionUtils.GetGetterMethodInfo(propertyInfo).IsPublic;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyOrField"/> class.
        /// </summary>
        /// <param name="fieldInfo"></param>
        public PropertyOrField(FieldInfo fieldInfo) : this((MemberInfo)fieldInfo)
        {
            _fieldInfo = fieldInfo;

            CanRead = true;
            CanWrite = true;
            IsStatic = fieldInfo.IsStatic;
            IsPublic = fieldInfo.IsPublic;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyOrField"/> class.
        /// </summary>
        /// <param name="memberInfo"></param>
        protected PropertyOrField(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;
            Base64Encoded = memberInfo.GetCustomAttribute<SerializeAsBase64Attribute>() != null;
            SerializeNull = memberInfo.GetCustomAttribute<SerializeNullAttribute>() != null;
            HasParameterAttribute = memberInfo.GetCustomAttribute<ParameterAttribute>() != null;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can read.
        /// </summary>
        public bool CanRead { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can write.
        /// </summary>
        public bool CanWrite { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is base64 encoded.
        /// </summary>
        public bool Base64Encoded { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance should serialize null values.
        /// </summary>
        public bool SerializeNull { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is static.
        /// </summary>
        public bool IsStatic { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is public.
        /// </summary>
        public bool IsPublic { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has a parameter attribute.
        /// </summary>
        public bool HasParameterAttribute { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is a field.
        /// </summary>
        public MemberInfo MemberInfo { get; private set; }

        /// <summary>
        /// Gets the value of the instance.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public object GetValue(object instance)
        {
            if (_propertyInfo != null)
                return _propertyInfo.GetValue(instance);
            if (_fieldInfo != null)
                return _fieldInfo.GetValue(instance);
            throw new InvalidOperationException("Property and Field cannot both be null");
        }

        /// <summary>
        /// Sets the value of the instance.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void SetValue(object instance, object value)
        {
            if (_propertyInfo != null)
            {
                _propertyInfo.SetValue(instance, value);
                return;
            }
            if (_fieldInfo != null)
            {
                _fieldInfo.SetValue(instance, value);
                return;
            }
            throw new InvalidOperationException("Property and Field cannot both be null");
        }

        /// <summary>
        /// Gets the name of the field in JSON.
        /// </summary>
        public string JsonFieldName
        {
            get { return MemberInfo.GetJsonFieldName(); }
        }

        /// <summary>
        /// Gets the name of the field in XML.
        /// </summary>
        public ReflectionUtils.GetDelegate GetDelegate
        {
            get
            {
                ReflectionUtils.GetDelegate getDelegate = null;
                if (_propertyInfo != null)
                {
                    getDelegate = ReflectionUtils.GetGetMethod(_propertyInfo);
                }
                if (_fieldInfo != null)
                {
                    getDelegate = ReflectionUtils.GetGetMethod(_fieldInfo);
                }


                if (getDelegate == null)
                {
                    throw new InvalidOperationException("Property and Field cannot both be null");
                }

                if (Base64Encoded)
                {
                    return delegate (object source)
                    {
                        var value = getDelegate(source);
                        var stringValue = value as string;
                        return stringValue == null ? value : stringValue.ToBase64String();
                    };
                }

                return getDelegate;
            }
        }

        /// <summary>
        /// Gets the set delegate.
        /// </summary>
        public ReflectionUtils.SetDelegate SetDelegate
        {
            get
            {
                ReflectionUtils.SetDelegate setDelegate = null;
                if (_propertyInfo != null)
                {
                    setDelegate = ReflectionUtils.GetSetMethod(_propertyInfo);
                }
                if (_fieldInfo != null)
                {
                    setDelegate = ReflectionUtils.GetSetMethod(_fieldInfo);
                }
                if (setDelegate == null)
                {
                    throw new InvalidOperationException("Property and Field cannot both be null");
                }
                if (Base64Encoded)
                {
                    return delegate (object source, object value)
                    {
                        var stringValue = value as string;
                        if (stringValue == null)
                        {
                            setDelegate(source, value);
                        }
                        setDelegate(source, stringValue.FromBase64String());
                    };
                }
                return setDelegate;
            }
        }

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        public Type Type
        {
            get
            {
                if (_propertyInfo != null)
                {
                    return _propertyInfo.PropertyType;
                }
                if (_fieldInfo != null)
                {
                    return _fieldInfo.FieldType;
                }
                throw new InvalidOperationException("Property and Field cannot both be null");
            }
        }

        /// <summary>
        /// Gets the field info.
        /// </summary>
        public bool CanDeserialize
        {
            get
            {
                return (IsPublic || HasParameterAttribute)
                    && !IsStatic
                    && CanWrite
                    && (_fieldInfo == null || !_fieldInfo.IsInitOnly);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can serialize.
        /// </summary>
        public bool CanSerialize
        {
            get { return IsPublic && CanRead && !IsStatic; }
        }
    }
}
