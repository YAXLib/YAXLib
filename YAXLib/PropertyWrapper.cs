// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Reflection;

namespace YAXLib;
internal sealed class PropertyWrapper: IMemberDescriptor
{
    private readonly PropertyInfo _wrappedProperty;
    public bool CanRead { get; }
    public bool CanWrite { get; }
    public bool IsPublic { get; }
    public MemberTypes MemberType => MemberTypes.Property;
    public string Name { get; }
    public Type Type { get; }

    public PropertyWrapper(PropertyInfo propertyInfo)
    {
        _wrappedProperty = propertyInfo;
        Name = propertyInfo.Name;
        Type = propertyInfo.PropertyType;
        CanRead = propertyInfo.CanRead;
        CanWrite = propertyInfo.CanWrite;
        IsPublic = ReflectionUtils.IsPublicProperty(propertyInfo);
    }

    public Attribute[] GetCustomAttributes()
    {
        return Attribute.GetCustomAttributes(_wrappedProperty);
    }

    public object? GetValue(object? obj, object[]? index = null)
    {
        return _wrappedProperty.GetValue(obj, index);
    }

    public void SetValue(object? obj, object? value, object[]? index = null)
    {
        _wrappedProperty.SetValue(obj, value, index);
    }
}
