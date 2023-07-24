// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.


using System;

namespace YAXLib.Enums;

/// <summary>
/// Enumerates different serialization options which could be set at construction time.
/// </summary>
[Flags]
public enum YAXSerializationOptions
{
    /// <summary>
    /// No serialization options set
    /// </summary>
    None = 0,

    /// <summary>
    /// Serializes null objects also (the default)
    /// </summary>
    SerializeNullObjects = 1,

    /// <summary>
    /// Prevents serialization of null objects.
    /// </summary>
    DontSerializeNullObjects = 2,

    /// <summary>
    /// Prevents that cycle references from child to parent objects cause an infinite loop.
    /// </summary>
    ThrowUponSerializingCyclingReferences = 4,

    /// <summary>
    /// Prevents serialization of properties with no setter
    /// </summary>
    DontSerializePropertiesWithNoSetter = 8,

    /// <summary>
    /// Never add YAXLib metadata attributes (e.g., 'yaxlib:realtype') to the serialized XML (even when they would be
    /// required for deserialization.)
    /// Useful when generating XML is targeting third party systems.
    /// </summary>
    SuppressMetadataAttributes = 16,

    /// <summary>
    /// Provides line number and position (where available) in deserialization exceptions.
    /// Enabling this has a performance impact
    /// </summary>
    DisplayLineInfoInExceptions = 32,

    /// <summary>
    /// Silently removes illegal XML characters when serializing, instead of throwing an exception.
    /// Note: XML containing illegal characters cannot be loaded and deserialized.
    /// <para>
    /// Default: disabled.
    /// </para>
    /// </summary>
    StripInvalidXmlChars = 64,

    /// <summary>
    /// Prevents serialization of default values.
    /// </summary>
    DoNotSerializeDefaultValues = 128
}
