// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Reflection;

namespace YAXLib;
internal class YaxFieldWrapper : IYaxFieldInfo
{
    private readonly FieldInfo _wrappedFieldInfo;
    public string Name { get; }
    public MemberTypes MemberType { get; }
    public bool IsPublic { get; }
    public Type Type { get; }

    public YaxFieldWrapper(FieldInfo fieldInfo)
    {
        _wrappedFieldInfo = fieldInfo;
        MemberType = fieldInfo.MemberType;
        IsPublic = fieldInfo.IsPublic;
        Type = fieldInfo.FieldType;
        Name = fieldInfo.Name;
    }

    public Attribute[] GetCustomAttributesByType(Type attrType, bool inherit)
    {
        return Attribute.GetCustomAttributes(_wrappedFieldInfo, attrType, inherit);
    }

    public Attribute[] GetCustomAttributes(bool inherit)
    {
        return Attribute.GetCustomAttributes(_wrappedFieldInfo, inherit);
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
