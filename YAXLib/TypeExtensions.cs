// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace YAXLib;

internal static class TypeExtensions
{
    /// <summary>
    /// Searches for the members defined for the current <see cref="Type" />,
    /// including the members from all <see cref="Type.BaseType" />s, using the specified binding constraints
    /// and <paramref name="includeBaseTypePrivateMembers" /> option.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="bindingFlags">
    /// Binding flags for the type. A bitwise combination of the enumeration values that specify how the search is conducted.
    /// -or-
    /// <see cref="BindingFlags.Default" /> to return an empty array.
    /// </param>
    /// <param name="includeBaseTypePrivateMembers">
    /// If set to <see langword="true" />, private fields and properties from base types are included in the search.
    /// </param>
    /// <returns>
    /// A list of <see cref="MemberInfo" /> objects representing all members defined for the current <see cref="Type" />
    /// and its base types that match the specified binding constraints and <paramref name="includeBaseTypePrivateMembers" />
    /// option.
    /// </returns>
    /// <remarks>
    /// Both, <see cref="Type.GetProperty(string, BindingFlags)" /> and <see cref="Type.GetMembers()" />
    /// return private properties/members.
    /// <see cref="Type.GetMembers(BindingFlags)" />, however, <b>does not</b> include members from <b>base types</b>,
    /// whereas <see cref="Type.GetProperty(string, BindingFlags)" /> also returns private properties of base types.
    /// </remarks>
    public static List<MemberInfo> GetMembers(this Type t, BindingFlags bindingFlags,
        bool includeBaseTypePrivateMembers)
    {
        var members = t.GetMembers(bindingFlags);
        var memberList = new List<MemberInfo>(members.Length * 2);
        memberList.AddRange(members);
        var currentType = t;
        while ((currentType = currentType.BaseType) != null && includeBaseTypePrivateMembers)
        {
            memberList.AddRange(currentType.GetMembers(bindingFlags));
        }

        return memberList;
    }
}