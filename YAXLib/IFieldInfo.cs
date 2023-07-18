// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

namespace YAXLib;

/// <summary>
/// Provides information about a field.
/// </summary>
public interface IFieldInfo : IMemberInfo
{
    /// <summary>
    /// Gets the value of the field for the specified object.
    /// </summary>
    /// <param name="obj">The object from which to retrieve the field value.</param>
    /// <returns>The value of the field.</returns>
    object? GetValue(object? obj);

    /// <summary>
    /// Sets the value of the field for the specified object.
    /// </summary>
    /// <param name="obj">The object on which to set the field value.</param>
    /// <param name="value">The value to be assigned to the field.</param>
    void SetValue(object? obj, object? value);
}
