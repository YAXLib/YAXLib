// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using YAXLib.Options;

namespace YAXLib;

/// <summary>
/// Resolves type information during serialization/deserialization.
/// </summary>
public interface ITypeInfoResolver
{
    /// <summary>
    /// Resolves the members to be serialized for the given type.
    /// </summary>
    /// <param name="proposedMembers">The list of proposed members to be serialized/deserialized.</param>
    /// <param name="type">The type for which members are to be resolved.</param>
    /// <param name="options">Serializer options for controlling the serialization process.</param>
    /// <returns>A list of IMemberInfo containing the resolved members to be serialized.</returns>
    IList<IMemberInfo> ResolveMembers(IList<IMemberInfo> proposedMembers, Type type, SerializerOptions options);

    /// <summary>
    /// Gets the custom type name for the given type during serialization.
    /// </summary>
    /// <param name="proposedName">The proposed name for the type.</param>
    /// <param name="type">The type for which the name is to be resolved.</param>
    /// <param name="options">Serializer options for controlling the serialization process.</param>
    /// <returns>A string representing the resolved custom type name.</returns>
    string GetTypeName(string proposedName, Type type, SerializerOptions options);
}
