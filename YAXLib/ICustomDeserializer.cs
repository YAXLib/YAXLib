// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

namespace YAXLib
{
    /// <summary>
    ///     Defines the interface to all custom deserializers used with YAXLib.
    ///     Note that normally you don't need to implement all the methods.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of field, property, class, or struct for which custom deserializer
    ///     is provided
    /// </typeparam>
    public interface ICustomDeserializer<T>
    {
    }
}