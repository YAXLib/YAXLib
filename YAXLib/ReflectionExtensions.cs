// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Reflection;

namespace YAXLib;
internal static class ReflectionExtensions
{
    public static IMemberDescriptor Wrap(this MemberInfo memberInfo)
    {
        return memberInfo.MemberType switch
        {
            MemberTypes.Field => new FieldWrapper((FieldInfo) memberInfo),
            MemberTypes.Property => new PropertyWrapper((PropertyInfo) memberInfo),
            _ => throw new ArgumentOutOfRangeException($"MemberType: {memberInfo.MemberType} is not supported. Property and Field are only supported."),
        };
    }
}
