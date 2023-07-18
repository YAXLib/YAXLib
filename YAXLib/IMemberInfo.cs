// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Reflection;

namespace YAXLib;

/// <summary>
/// Provides information about a member.
/// </summary>
public interface IMemberInfo
{
    /// <summary>
    /// Gets the name of the member.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the type of the member.
    /// </summary>
    MemberTypes MemberType { get; }

    /// <summary>
    /// Gets a value indicating whether the member is public.
    /// </summary>
    bool IsPublic { get; }

    /// <summary>
    /// Gets the data type of the member.
    /// </summary>
    Type Type { get; }

    /// <summary>
    /// Retrieves an array of custom attributes applied to the member.
    /// </summary>
    /// <param name="attrType">The type of the custom attribute to retrieve.</param>
    /// <param name="inherit">Specifies whether to search for attributes in the inheritance chain.</param>
    /// <returns>An array of custom attributes of the specified type.</returns>
    Attribute[] GetCustomAttributes(Type attrType, bool inherit);

    /// <summary>
    /// Retrieves an array of all custom attributes applied to the member.
    /// </summary>
    /// <param name="inherit">Specifies whether to search for attributes in the inheritance chain.</param>
    /// <returns>An array of all custom attributes applied to the member.</returns>
    Attribute[] GetCustomAttributes(bool inherit);
}
