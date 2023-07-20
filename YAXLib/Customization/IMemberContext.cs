// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

namespace YAXLib.Customization;

/// <summary>
/// The member context interface provides information about the attributes of a member and member metadata.
/// </summary>
public interface IMemberContext
{
    /// <summary>
    /// The member's <see cref="IMemberDescriptor" /> for member serialization, else <see langword="null" />.
    /// </summary>
    IMemberDescriptor MemberInfo { get; }

    /// <summary>
    /// The member's <see cref="Customization.TypeContext" /> for member serialization./>.
    /// </summary>
    TypeContext TypeContext { get; }

    /// <summary>
    /// Gets value of this member in the specified object.
    /// </summary>
    /// <param name="obj">The object from which the value must be retrieved.</param>
    /// <returns>The value for this member.</returns>
    object? GetValue(object? obj);
}
