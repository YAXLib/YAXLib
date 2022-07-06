// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

namespace YAXLib
{
    public interface IYAXMultiStateDeserializer<T> : IYAXDeserializer<T>
    {
        /// <summary>
        /// Sets the object used as the base object in the next stage of de-serialization.
        /// This method enables multi-stage de-serialization for YAXLib.
        /// </summary>
        /// <param name="obj">The object used as the base object in the next stage of de-serialization.</param>
        void SetDeserializationBaseObject(T obj);
    }
}