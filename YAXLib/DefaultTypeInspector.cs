// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Reflection;
using YAXLib.Options;

namespace YAXLib;

/// <inheritdoc />
public class DefaultTypeInspector : ITypeInspector
{
    internal static ITypeInspector Instance { get; } = new DefaultTypeInspector();

    /// <inheritdoc />
    public virtual IEnumerable<IMemberDescriptor> GetMembers(Type type, SerializerOptions options, bool includePrivateMembersFromBaseTypes)
    {
#pragma warning disable S3011 // disable sonar accessibility bypass warning
        foreach (var member in type.GetMembers(
                     BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                     includePrivateMembersFromBaseTypes))
#pragma warning restore S3011 // enable sonar accessibility bypass warning
        {
            if (!IsValidPropertyOrField(member)) continue;
            if (member is PropertyInfo prop && !CanSerializeProperty(prop)) continue;

            if ((ReflectionUtils.IsCollectionType(type) || ReflectionUtils.IsIDictionary(type))
                && ReflectionUtils.IsPartOfNetFx(member))
                continue;

            yield return member.Wrap();
        }
    }

    /// <inheritdoc />
    public virtual string GetTypeName(Type type, SerializerOptions options)
    {
        return ReflectionUtils.GetTypeFriendlyName(type);
    }

    private static bool IsValidPropertyOrField(MemberInfo member)
    {
        // Exclude names of compiler-generated backing fields like "<my_member>k__BackingField"
        var name0 = member.Name[0];
        return (char.IsLetter(name0) || name0 == '_') &&
               (member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field);
    }

    private static bool CanSerializeProperty(PropertyInfo prop)
    {
        // ignore indexers; if member is an indexer property, do not serialize it
        if (prop.GetIndexParameters().Length > 0)
            return false;

        // don't serialize delegates as well
        if (ReflectionUtils.IsTypeEqualOrInheritedFromType(prop.PropertyType, typeof(Delegate)))
            return false;

        return true;
    }
}
