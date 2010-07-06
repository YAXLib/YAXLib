// Copyright 2009 - 2010 Sina Iravanian - <sina@sinairv.com>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace YAXLib
{
    /// <summary>
    /// Defines the interface to all custom deserializers used with YAXLib.
    /// Note that normally you don't need to implement all the methods.
    /// </summary>
    /// <typeparam name="T">The type of field, property, class, or struct for which custom deserializer
    /// is provided</typeparam>
    public interface ICustomDeserializer<T>
    {
        /// <summary>
        /// Deserializes from an xml attribute, and returns the retreived value.
        /// You will normally need to use XAttribute.Value property only.
        /// </summary>
        /// <param name="attrib">The attribute to deserialize.</param>
        /// <returns></returns>
        T DeserializeFromAttribute(XAttribute attrib);

        /// <summary>
        /// Deserializes from an xml element, and returns the retreived value.
        /// You might need to use XElement.Value, XElement.Nodes(), 
        /// XElement.Attributes(), and XElement.Elements() only.
        /// </summary>
        /// <param name="element">The element to deserialize.</param>
        /// <returns></returns>
        T DeserializeFromElement(XElement element);

        /// <summary>
        /// Deserializes from a string value which has been serialized as the content of an element
        /// </summary>
        /// <param name="value">The string value to deserialize.</param>
        /// <returns></returns>
        T DeserializeFromValue(string value);
    }
}
