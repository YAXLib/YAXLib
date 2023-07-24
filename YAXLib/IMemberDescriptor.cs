// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Reflection;

namespace YAXLib;

/// <summary>
/// Provides information about a member.
/// </summary>
public interface IMemberDescriptor
{
    /// <summary>
    /// Gets a value indicating whether the member can be read.
    /// </summary>
    bool CanRead { get; }

    /// <summary>
    /// Gets a value indicating whether the member can be written to.
    /// </summary>
    bool CanWrite { get; }

    /// <summary>
    /// Gets a value indicating whether the member is public.
    /// </summary>
    bool IsPublic { get; }

    /// <summary>
    /// Gets the type of the member.
    /// </summary>
    MemberTypes MemberType { get; }

    /// <summary>
    /// Gets the name of the member.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the data type of the member.
    /// </summary>
    Type Type { get; }

    /// <summary>
    /// Retrieves an array of all custom attributes applied to the member.
    /// </summary>
    /// <returns>An array of all custom attributes applied to the member.</returns>
    Attribute[] GetCustomAttributes();

    /// <summary>
    /// Gets the value of the member for the specified object.
    /// </summary>
    /// <param name="obj">The object from which to retrieve the member value.</param>
    /// <param name="index">Optional index parameters for indexed properties.</param>
    /// <returns>The value of the member.</returns>
    object? GetValue(object? obj, object[]? index = null);

    /// <summary>
    /// Sets the value of the member for the specified object.
    /// </summary>
    /// <param name="obj">The object on which to set the member value.</param>
    /// <param name="value">The value to be assigned to the member.</param>
    /// <param name="index">Optional index parameters for indexed properties.</param>
    void SetValue(object? obj, object? value, object[]? index = null);
}
