// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.IO;
using System.Xml;
using System.Xml.Linq;
using YAXLib.Options;

namespace YAXLib
{
    /// <summary>
    /// Serialize <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IYAXSerializer<in T>
    {
        /// <summary>
        ///     Gets the <see cref="SerializerOptions"/> settings
        ///     to influence the process of serialization or de-serialization of <see cref="YAXSerializer"/>s.
        /// </summary>
        SerializerOptions Options { get; }

        /// <summary>
        ///     Gets the parsing errors.
        /// </summary>
        /// <value>The parsing errors.</value>
        YAXParsingErrors ParsingErrors { get; }

        /// <summary>
        ///     Serializes the specified object and returns a string containing the XML.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A <code>System.String</code> containing the XML</returns>
        string Serialize(T obj);

        /// <summary>
        ///     Serializes the specified object into a <c>TextWriter</c> instance.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="textWriter">The <c>TextWriter</c> instance.</param>
        void Serialize(T obj, TextWriter textWriter);

        /// <summary>
        ///     Serializes the specified object into a <c>XmlWriter</c> instance.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="xmlWriter">The <c>XmlWriter</c> instance.</param>
        void Serialize(T obj, XmlWriter xmlWriter);

        /// <summary>
        ///     Serializes the specified object and returns an instance of <c>XDocument</c> containing the result.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>An instance of <c>XDocument</c> containing the resulting XML</returns>
        XDocument SerializeToXDocument(T obj);

        /// <summary>
        ///     Serializes the specified object to file.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="fileName">Path to the file.</param>
        void SerializeToFile(T obj, string fileName);
    }
}