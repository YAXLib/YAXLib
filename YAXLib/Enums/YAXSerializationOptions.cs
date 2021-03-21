// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib
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
        ///     TODO: update the comment
        ///     Prevents serialization of child objects that refer to a parent object which is already serialized, and hene causing
        ///     a cycle or infinite loop
        /// </summary>
        ThrowUponSerializingCyclingReferences = 2,

        /// <summary>
        ///     Prevents serailization of properties with no setter
        /// </summary>
        DontSerializePropertiesWithNoSetter = 4,

        /// <summary>
        ///     Never add YAXLib metadata attributes (e.g., 'yaxlib:realtype') to the serialized XML (even when they would be
        ///     required for deserialization.)
        ///     Useful when generating XML intended for another system's consumption.
        /// </summary>
        SuppressMetadataAttributes = 8,

        /// <summary>
        ///     Provides line number and position (where available) in deserialization exceptions.
        ///     Enabling this may have a performance impact
        /// </summary>
        DisplayLineInfoInExceptions = 16
    }
}