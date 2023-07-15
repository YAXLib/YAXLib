// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Reflection;

namespace YAXLib;
internal static class ReflectionExtensions
{
    public static IYaxMemberInfo ToYaxMemberInfo(this MemberInfo memberInfo)
    {
        switch (memberInfo.MemberType)
        {
            case MemberTypes.Field:
               return new YaxFieldWrapper((FieldInfo) memberInfo);
            case MemberTypes.Property:
                var property = (PropertyInfo)memberInfo;
                return new YaxPropertyWrapper(property);
            default:
                throw new ArgumentOutOfRangeException($"MemberType: {memberInfo.MemberType} is not supported. Property and Field are only supported.");
        }
    }
}
