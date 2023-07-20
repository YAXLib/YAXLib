// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Reflection;

namespace YAXLib;

internal sealed class FieldWrapper : IMemberDescriptor
{
    private readonly FieldInfo _wrappedFieldInfo;
    public bool CanRead => true;
    public bool CanWrite => true;
    public bool IsPublic { get; }
    public MemberTypes MemberType => MemberTypes.Field;
    public string Name { get; }
    public Type Type { get; }

    public FieldWrapper(FieldInfo fieldInfo)
    {
        _wrappedFieldInfo = fieldInfo;
        IsPublic = fieldInfo.IsPublic;
        Type = fieldInfo.FieldType;
        Name = fieldInfo.Name;
    }

    public Attribute[] GetCustomAttributes()
    {
        return Attribute.GetCustomAttributes(_wrappedFieldInfo);
    }

    public object? GetValue(object? obj)
    {
        return _wrappedFieldInfo.GetValue(obj);
    }

    public void SetValue(object? obj, object? value)
    {
        _wrappedFieldInfo.SetValue(obj, value);
    }
}
