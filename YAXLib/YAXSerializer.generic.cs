// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.IO;
using System.Xml;
using System.Xml.Linq;
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

        /// <summary>
        ///     Serializes the specified object and returns a string containing the XML.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A <code>System.String</code> containing the XML</returns>
        public string Serialize(T obj)
        {
            return base.Serialize(obj).ToString();
        }

        /// <summary>
        ///     Serializes the specified object and returns an instance of <c>XDocument</c> containing the result.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>An instance of <c>XDocument</c> containing the resulting XML</returns>
        public XDocument SerializeToXDocument(T obj)
        {
            return base.SerializeToXDocument(obj);
        }

        /// <summary>
        ///     Serializes the specified object into a <c>TextWriter</c> instance.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="textWriter">The <c>TextWriter</c> instance.</param>
        public void Serialize(T obj, TextWriter textWriter)
        {
            base.Serialize(obj, textWriter);
        }

        /// <summary>
        ///     Serializes the specified object into a <c>XmlWriter</c> instance.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="xmlWriter">The <c>XmlWriter</c> instance.</param>
        public void Serialize(T obj, XmlWriter xmlWriter)
        {
            base.Serialize(obj, xmlWriter);
        }

        /// <summary>
        ///     Serializes the specified object to file.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="fileName">Path to the file.</param>
        public void SerializeToFile(T obj, string fileName)
        {
            base.SerializeToFile(obj, fileName);
        }

        /// <summary>
        ///     Deserializes the specified string containing the XML serialization and returns an object.
        /// </summary>
        /// <param name="input">The input string containing the XML serialization.</param>
        /// <returns>The deserialized object.</returns>
        public new T Deserialize(string input)
        {
            return (T)base.Deserialize(input);
        }

        /// <summary>
        ///     Deserializes an object while reading input from an instance of <c>XmlReader</c>.
        /// </summary>
        /// <param name="xmlReader">The <c>XmlReader</c> instance to read input from.</param>
        /// <returns>The deserialized object.</returns>
        public new T Deserialize(XmlReader xmlReader)
        {
            return (T) base.Deserialize(xmlReader);
        }

        /// <summary>
        ///     Deserializes an object while reading input from an instance of <c>TextReader</c>.
        /// </summary>
        /// <param name="textReader">The <c>TextReader</c> instance to read input from.</param>
        /// <returns>The deserialized object.</returns>
        public new T Deserialize(TextReader textReader)
        {
            return (T) base.Deserialize(textReader);
        }

        /// <summary>
        ///     Deserializes an object while reading from an instance of <c>XElement</c>
        /// </summary>
        /// <param name="element">The <c>XElement</c> instance to read from.</param>
        /// <returns>The deserialized object</returns>
        public new T Deserialize(XElement element)
        {
            return (T) base.Deserialize(element);
        }

        /// <summary>
        ///     Deserializes an object from the specified file which contains the XML serialization of the object.
        /// </summary>
        /// <param name="fileName">Path to the file.</param>
        /// <returns>The deserialized object.</returns>
        public new T DeserializeFromFile(string fileName)
        {
            return (T) base.Deserialize(fileName);
        }

        /// <summary>
        ///     Sets the object used as the base object in the next stage of de-serialization.
        ///     This method enables multi-stage de-serialization for YAXLib.
        /// </summary>
        /// <param name="obj">The object used as the base object in the next stage of de-serialization.</param>
        public void SetDeserializationBaseObject(T obj)
        {
            base.SetDeserializationBaseObject(obj);
        }
    }
}
