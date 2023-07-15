// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Reflection;

namespace YAXLib;
public class YaxPropertyWrapper: IYaxPropertyInfo
{
    private readonly PropertyInfo _wrappedProperty;
    public string Name { get; }
    public MemberTypes MemberType { get; }
    public bool IsPublic { get; }
    public Type Type { get; }
    public bool CanRead { get; }
    public bool CanWrite { get; }

    public YaxPropertyWrapper(PropertyInfo propertyInfo)
    {
        _wrappedProperty = propertyInfo;
        Name = propertyInfo.Name;
        MemberType = propertyInfo.MemberType;
        Type = propertyInfo.PropertyType;
        CanRead = propertyInfo.CanRead;
        CanWrite = propertyInfo.CanWrite;
        IsPublic = ReflectionUtils.IsPublicProperty(propertyInfo);
    }

    public Attribute[] GetCustomAttributes(Type attrType, bool inherit)
    {
        return Attribute.GetCustomAttributes(_wrappedProperty, attrType, inherit);
    }

    public Attribute[] GetCustomAttributes(bool inherit)
    {
        return Attribute.GetCustomAttributes(_wrappedProperty, inherit);
    }

    public object? GetValue(object? obj, object[]? index)
    {
        return _wrappedProperty.GetValue(obj, index);
    }

    public void SetValue(object? obj, object? value, object[]? index)
    {
        _wrappedProperty.SetValue(obj, value, index);
    }
}
