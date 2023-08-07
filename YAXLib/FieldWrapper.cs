// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Reflection;

namespace YAXLib;

internal sealed class FieldWrapper : IMemberDescriptor
{
    internal FieldInfo WrappedField { get; }
    public bool CanRead => true;
    public bool CanWrite => true;
    public bool IsPublic { get; }
    public MemberTypes MemberType => MemberTypes.Field;
    public string Name { get; }
    public Type Type { get; }

    public FieldWrapper(FieldInfo fieldInfo)
    {
        WrappedField = fieldInfo;
        IsPublic = fieldInfo.IsPublic;
        Type = fieldInfo.FieldType;
        Name = fieldInfo.Name;
    }

    public Attribute[] GetCustomAttributes()
    {
        return Attribute.GetCustomAttributes(WrappedField);
    }

    public object? GetValue(object? obj, object[]? index = null)
    {
        return WrappedField.GetValue(obj);
    }

    public void SetValue(object? obj, object? value, object[]? index = null)
    {
        WrappedField.SetValue(obj, value);
    }

    public override string? ToString()
    {
        return WrappedField.ToString();
    }
}
