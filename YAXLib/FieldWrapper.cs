// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Reflection;

namespace YAXLib;

internal sealed class FieldWrapper : IMemberDescriptor
{
    private readonly FieldInfo _wrappedField;
    public bool CanRead => true;
    public bool CanWrite => true;
    public bool IsPublic { get; }
    public MemberTypes MemberType => MemberTypes.Field;
    public string Name { get; }
    public Type Type { get; }

    public FieldWrapper(FieldInfo fieldInfo)
    {
        _wrappedField = fieldInfo;
        IsPublic = fieldInfo.IsPublic;
        Type = fieldInfo.FieldType;
        Name = fieldInfo.Name;
    }

    public Attribute[] GetCustomAttributes()
    {
        return Attribute.GetCustomAttributes(_wrappedField);
    }

    public object? GetValue(object? obj, object[]? index = null)
    {
        return _wrappedField.GetValue(obj);
    }

    public void SetValue(object? obj, object? value, object[]? index = null)
    {
        _wrappedField.SetValue(obj, value);
    }
}
