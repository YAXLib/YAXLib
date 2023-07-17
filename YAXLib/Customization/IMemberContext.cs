// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

namespace YAXLib.Customization;

/// <summary>
/// The member context interface provides information about the attributes of a member and member metadata.
/// </summary>
public interface IMemberContext
{
    /// <summary>
    /// The member's <see cref="IMemberInfo" /> for member serialization, else <see langword="null" />.
    /// </summary>
    IMemberInfo MemberInfo { get; }

    /// <summary>
    /// The member's <see cref="IFieldInfo" /> for field serialization, else <see langword="null" />.
    /// </summary>
    IFieldInfo? FieldInfo { get; }

    /// <summary>
    /// The member's <see cref="IPropertyInfo" /> for property serialization, else <see langword="null" />.
    /// </summary>
    IPropertyInfo? PropertyInfo { get; }

    /// <summary>
    /// The member's <see cref="Customization.TypeContext" /> for member serialization./>.
    /// </summary>
    TypeContext TypeContext { get; }

    /// <summary>
    /// Gets value of this member in the specified object.
    /// </summary>
    /// <param name="obj">The object from which the value must be retrieved.</param>
    /// <param name="index">
    /// Optional index values for indexed properties.
    /// The indexes of indexed properties are zero-based. This value should be <see langword="null" /> for non-indexed
    /// properties.
    /// </param>
    /// <returns>The value for this member.</returns>
    object? GetValue(object? obj, object[]? index = null);
}
