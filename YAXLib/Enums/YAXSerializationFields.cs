// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.


using System;
using YAXLib.Attributes;

namespace YAXLib.Enums;

/// <summary>
/// Enumerates possible options for a serializable type
/// </summary>
public enum YAXSerializationFields
{
    /// <summary>
    /// Serializes only the public properties (the default behaviour)
    /// </summary>
    PublicPropertiesOnly,

    /// <summary>
    /// Serializes all fields (properties or fields) which are <see langword="public" />, or non-public.
    /// <b>Note</b>: To also include private <b>fields</b> and <b>properties</b> from <b><see cref="Type.BaseType" />s</b>,
    /// <see cref="YAXSerializableTypeAttribute.IncludePrivateMembersFromBaseTypes" /> must be set to <see langword="true" />.
    /// </summary>
    AllFields,

    /// <summary>
    /// Serializes all fields (properties or member variables) which are public, or non-public, if attributed as
    /// <c>YAXSerializableField</c>
    /// </summary>
    AttributedFieldsOnly
}