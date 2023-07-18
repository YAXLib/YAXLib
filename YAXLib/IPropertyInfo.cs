// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

namespace YAXLib;

/// <summary>
/// Provides information about a property.
/// </summary>
public interface IPropertyInfo : IMemberInfo
{
    /// <summary>
    /// Gets a value indicating whether the property can be read.
    /// </summary>
    bool CanRead { get; }

    /// <summary>
    /// Gets a value indicating whether the property can be written to.
    /// </summary>
    bool CanWrite { get; }

    /// <summary>
    /// Gets the value of the property for the specified object and optional index parameters.
    /// </summary>
    /// <param name="obj">The object from which to retrieve the property value.</param>
    /// <param name="index">Optional index parameters for indexed properties.</param>
    /// <returns>The value of the property.</returns>
    object? GetValue(object? obj, object[]? index);

    /// <summary>
    /// Sets the value of the property for the specified object and optional index parameters.
    /// </summary>
    /// <param name="obj">The object on which to set the property value.</param>
    /// <param name="value">The value to be assigned to the property.</param>
    /// <param name="index">Optional index parameters for indexed properties.</param>
    void SetValue(object? obj, object? value, object[]? index);
}
