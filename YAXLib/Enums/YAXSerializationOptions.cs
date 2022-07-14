// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib.Enums
{
    /// <summary>
    ///     Enumerates different serialization options which could be set at construction time.
    /// </summary>
    [Flags]
    public enum YAXSerializationOptions
    {
        /// <summary>
        ///     Serializes null objects also (the default)
        /// </summary>
        SerializeNullObjects = 0,

        /// <summary>
        ///     Prevents serialization of null objects.
        /// </summary>
        DontSerializeNullObjects = 1,

        /// <summary>
        ///     Prevents that cycle references from child to parent objects cause an infinite loop.
        /// </summary>
        ThrowUponSerializingCyclingReferences = 2,

        /// <summary>
        ///     Prevents serialization of properties with no setter
        /// </summary>
        DontSerializePropertiesWithNoSetter = 4,

        /// <summary>
        ///     Never add YAXLib metadata attributes (e.g., 'yaxlib:realtype') to the serialized XML (even when they would be
        ///     required for deserialization.)
        ///     Useful when generating XML is targeting third party systems.
        /// </summary>
        SuppressMetadataAttributes = 8,

        /// <summary>
        ///     Provides line number and position (where available) in deserialization exceptions.
        ///     Enabling this has a performance impact
        /// </summary>
        DisplayLineInfoInExceptions = 16,
    }
}