// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Xml.Linq;
using YAXLib.Customization;

namespace YAXLib;

/// <summary>
/// Defines the interface to all custom serializers and deserializers.
/// Note that normally you don't need to implement all the methods.
/// </summary>
/// <typeparam name="T">
/// The type of field, property, class, or struct for which the custom serializer is provided.
/// </typeparam>
public interface ICustomSerializer<T>
{
    /// <summary>
    /// Serializes the given object and fills the provided reference to the
    /// XML attribute appropriately.
    /// <para>
    /// If the name of the provided attribute is changed, the custom serializer cannot be invoked when deserializing.
    /// </para>
    /// </summary>
    /// <param name="objectToSerialize">The object to serialize.</param>
    /// <param name="attrToFill">
    /// The XML attribute to fill. If the name of the provided attribute is changed, the custom
    /// serializer cannot be invoked when deserializing.
    /// </param>
    /// <param name="serializationContext">
    /// Contains information about the type and members of the
    /// <paramref name="objectToSerialize" />.
    /// </param>
    void SerializeToAttribute(T objectToSerialize, XAttribute attrToFill,
        ISerializationContext serializationContext);

    /// <summary>
    /// Serializes the given object and fills the provided reference to the
    /// XML element appropriately.
    /// <para>
    /// If the name of the provided element is changed, the custom serializer cannot be invoked when deserializing.
    /// </para>
    /// </summary>
    /// <param name="objectToSerialize">The object to serialize.</param>
    /// <param name="elemToFill">
    /// The XML element to fill. If the name of the provided element is changed, the custom serializer
    /// cannot be invoked when deserializing.
    /// </param>
    /// <param name="serializationContext">
    /// Contains information about the type and members of the
    /// <paramref name="objectToSerialize" />.
    /// </param>
    void SerializeToElement(T objectToSerialize, XElement elemToFill, ISerializationContext serializationContext);

    /// <summary>
    /// Serializes the given object to an string to be used as a value for an
    /// XML element.
    /// </summary>
    /// <param name="objectToSerialize">The object to serialize.</param>
    /// <param name="serializationContext">
    /// Contains information about the type and members of the
    /// <paramref name="objectToSerialize" />.
    /// </param>
    /// <returns></returns>
    string SerializeToValue(T objectToSerialize, ISerializationContext serializationContext);

    /// <summary>
    /// Deserializes from an xml attribute, and returns the retrieved value.
    /// You will normally need to use XAttribute.Value property only.
    /// </summary>
    /// <param name="attribute">The attribute to deserialize.</param>
    /// <param name="serializationContext">Contains information about the type and members of the object to deserialize.</param>
    /// <returns></returns>
    T? DeserializeFromAttribute(XAttribute attribute, ISerializationContext serializationContext);

    /// <summary>
    /// Deserializes from an xml element, and returns the retrieved value.
    /// You might need to use XElement.Value, XElement.Nodes(),
    /// XElement.Attributes(), and XElement.Elements() only.
    /// </summary>
    /// <param name="element">The element to deserialize.</param>
    /// <param name="serializationContext">Contains information about the type and members of the object to deserialize.</param>
    /// <returns></returns>
    T? DeserializeFromElement(XElement element, ISerializationContext serializationContext);

    /// <summary>
    /// Deserializes from a string value which has been serialized as the content of an element
    /// </summary>
    /// <param name="value">The string value to deserialize.</param>
    /// <param name="serializationContext">Contains information about the type and members of the object to deserialize.</param>
    /// <returns></returns>
    T? DeserializeFromValue(string value, ISerializationContext serializationContext);
}
