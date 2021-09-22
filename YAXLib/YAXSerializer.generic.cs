// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Options;

namespace YAXLib
{
    /// <summary>
    ///     An XML serialization class which lets developers design the XML file structure and select the exception handling
    ///     policy.
    ///     This class also supports serializing most of the collection classes such as the Dictionary generic class.
    /// </summary>
    public class YAXSerializer<T> : YAXSerializer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXSerializer" /> class.
        /// </summary>
        public YAXSerializer() : base(typeof(T))
        {

        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXSerializer" /> class.
        /// </summary>
        /// <param name="options">The <see cref="SerializerOptions"/> settings to influence the process of serialization or de-serialization</param>
        public YAXSerializer(SerializerOptions options) : base(typeof(T), options)
        {

        }
    }
}
