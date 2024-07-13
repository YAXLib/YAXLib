﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using YAXLib.Attributes;
using YAXLib.Exceptions;
using YAXLib.Pooling.SpecializedPools;

namespace YAXLib;

/// <summary>
/// Provides a wrapper around enum types, in order to provide alias definition capabilities for enums
/// </summary>
internal class EnumWrapper
{
    private static readonly char[] SplitChars = [','];
    private static readonly char[] AliasNameSeparators = [',', ' ', '\t'];

    /// <summary>
    /// Maps real enum names to their corresponding user defined aliases
    /// </summary>
    private readonly Dictionary<string, string> _enumMembers = new();

    /// <summary>
    /// The enum underlying type
    /// </summary>
    private readonly Type _enumType;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumWrapper" /> class.
    /// </summary>
    /// <param name="t">The enum type.</param>
    public EnumWrapper(Type t)
    {
        if (!t.IsEnum)
            throw new ArgumentException($"The type of '{nameof(t)}' is not an Enum", nameof(t));

        _enumType = t;

        foreach (var m in t.GetFields())
            if (m.FieldType == t)
            {
                var originalName = m.Name;
                var alias = originalName;
                foreach (var attr in m.GetCustomAttributes(false))
                    if (attr is YAXEnumAttribute enumAttr)
                        alias = enumAttr.Alias;

                if (alias == originalName) continue;

                if (!_enumMembers.ContainsKey(originalName) && !_enumMembers.ContainsValue(alias))
                    _enumMembers.Add(m.Name, alias);
                else
                    throw new YAXEnumAliasException(
                        $"Enum alias '{alias}' for original name '{originalName}' already exists.");
            }
    }

    /// <summary>
    /// Parses the alias and returns the correct enum value. Throws an exception if the alias cannot be matched.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <returns>the enum member corresponding to the specified alias</returns>
    public object ParseAlias(string alias)
    {
        var components = alias.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);

        if (components.Length > 0)
        {
            using var pooledObject = StringBuilderPool.Instance.Get(out var sb);
            var realName = FindRealNameFromAlias(components[0]);
            sb.Append(realName);

            for (var i = 1; i < components.Length; i++)
            {
                sb.Append(", ");
                realName = FindRealNameFromAlias(components[i]);
                sb.Append(realName);
            }

            return Enum.Parse(_enumType, sb.ToString());
        }

        throw new ArgumentException("Invalid enum alias", nameof(alias));
    }

    /// <summary>
    /// Gets the alias for the specified enum value.
    /// </summary>
    /// <param name="enumMember">The enum member.</param>
    /// <returns>the alias corresponding to the specified enum member</returns>
    public object GetAlias(object enumMember)
    {
        var originalName = enumMember.ToString()!;

        var components = originalName.Split(AliasNameSeparators, StringSplitOptions.RemoveEmptyEntries);

        if (components.Length == 1)
        {
            return _enumMembers.TryGetValue(originalName, out var alias)
                ? alias
                : enumMember;
        }

        if (components.Length > 1)
        {
            using var pooledObject = StringBuilderPool.Instance.Get(out var result);
            for (var i = 0; i < components.Length; i++)
            {
                if (i != 0)
                    result.Append(", ");

                result.Append(_enumMembers.TryGetValue(components[i], out var alias)
                    ? alias
                    : components[i]);
            }

            return result.ToString();
        }

        throw new ArgumentException("Invalid enum member", nameof(enumMember));
    }

    /// <summary>
    /// Finds the real name from alias.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <returns>the real name of the enum member</returns>
    private string FindRealNameFromAlias(string alias)
    {
        alias = alias.Trim();
        foreach (var pair in _enumMembers)
            if (pair.Value == alias)
                return pair.Key;

        return alias;
    }
}
