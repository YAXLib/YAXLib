// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Reflection;

namespace YAXLib;
internal sealed class PropertyWrapper: IMemberDescriptor
{
    internal PropertyInfo WrappedProperty { get; }
    public bool CanRead { get; }
    public bool CanWrite { get; }
    public bool IsPublic { get; }
    public MemberTypes MemberType => MemberTypes.Property;
    public string Name { get; }
    public Type Type { get; }

    public PropertyWrapper(PropertyInfo propertyInfo)
    {
        WrappedProperty = propertyInfo;
        Name = propertyInfo.Name;
        Type = propertyInfo.PropertyType;
        CanRead = propertyInfo.CanRead;
        CanWrite = propertyInfo.CanWrite;
        IsPublic = ReflectionUtils.IsPublicProperty(propertyInfo);
    }

    public Attribute[] GetCustomAttributes()
    {
        return Attribute.GetCustomAttributes(WrappedProperty);
    }

    public object? GetValue(object? obj, object[]? index = null)
    {
        return WrappedProperty.GetValue(obj, index);
    }

    public void SetValue(object? obj, object? value, object[]? index = null)
    {
        WrappedProperty.SetValue(obj, value, index);
    }

    public override string? ToString()
    {
        return WrappedProperty.ToString();
    }
}
