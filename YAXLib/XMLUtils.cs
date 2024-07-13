// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using YAXLib.Exceptions;

namespace YAXLib;

/// <summary>
/// Provides utility methods for manipulating XML.
/// There are four methods for each unit. UnitExists, FindUnit, CanCreateUnit, CreateUnit
/// Units are: Location, Element, and Attribute
/// </summary>
internal static class XMLUtils
{
    /// <summary>
    /// Determines whether the location specified exists in the given XML element.
    /// </summary>
    /// <param name="baseElement">The XML element.</param>
    /// <param name="location">The location string.</param>
    /// <returns>a value indicating whether the location specified exists in the given XML element</returns>
    public static bool LocationExists(XElement baseElement, string location)
    {
        var newLoc = FindLocation(baseElement, location);
        return newLoc != null;
    }

    /// <summary>
    /// Finds the location specified in the given XML element specified.
    /// </summary>
    /// <param name="baseElement">The XML element.</param>
    /// <param name="location">The location string.</param>
    /// <returns>the XML element corresponding to the specified location, or <c>null</c> if it is not found</returns>
    public static XElement? FindLocation(XElement baseElement, string location)
    {
        if (baseElement == null)
            throw new ArgumentNullException(nameof(baseElement));

        if (location == null)
            throw new ArgumentNullException(nameof(location));

        var locSteps = location.SplitPathNamespaceSafe();

        var currentLocation = baseElement;
        foreach (var loc in locSteps)
            if (loc == ".")
            {
                // nothing to do
            }
            else if (loc == "..")
            {
                currentLocation = currentLocation.Parent;
                if (currentLocation == null)
                    break;
            }
            else
            {
                XName curLocName = loc;
                if (curLocName.Namespace.IsEmpty())
                    currentLocation = currentLocation.Element(curLocName);
                else
                    currentLocation = currentLocation.Element_NamespaceNeutral(curLocName);

                if (currentLocation == null)
                    break;
            }

        return currentLocation;
    }

    /// <summary>
    /// Strips all invalid characters from the input value, if <paramref name="enabled" /> is <see langword="true" />.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="enabled"></param>
    /// <returns></returns>
    public static string StripInvalidXmlChars(this string? input, bool enabled)
    {
        if (!enabled || input == null) return input ?? string.Empty;

        var buffer = ArrayPool<char>.Shared.Rent(input.Length);

        try
        {
            var written = 0;

            foreach (var c in input.Where(XmlConvert.IsXmlChar))
            {
                buffer[written++] = c;
            }

            return written == input.Length
                ? input
                : buffer.AsSpan(0, written).ToString();
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    /// <summary>
    /// Determines whether the specified location can be created in the specified XML element.
    /// </summary>
    /// <param name="baseElement">The XML element.</param>
    /// <param name="location">The location string.</param>
    /// <returns>
    /// <c>true</c> if the specified location can be created in the specified XML element; otherwise, <c>false</c>.
    /// </returns>
    public static bool CanCreateLocation(XElement baseElement, string location)
    {
        var locSteps = location.SplitPathNamespaceSafe();

        var currentLocation = baseElement;
        foreach (var loc in locSteps)
            if (loc == ".")
            {
                // nothing to do
            }
            else if (loc == "..")
            {
                currentLocation = currentLocation.Parent;
                if (currentLocation == null)
                    return false;
            }
            else
            {
                XName curLocName = loc;
                if (curLocName.Namespace.IsEmpty())
                    currentLocation = currentLocation.Element(curLocName);
                else
                    currentLocation = currentLocation.Element_NamespaceNeutral(curLocName);

                if (currentLocation == null)
                    return true;
            }

        return true;
    }

    /// <summary>
    /// Creates and returns XML element corresponding to the specified location in the given XML element.
    /// </summary>
    /// <param name="baseElement">The XML element.</param>
    /// <param name="location">The location string.</param>
    /// <returns>XML element corresponding to the specified location created in the given XML element</returns>
    public static XElement CreateLocation(XElement baseElement, string location)
    {
        var locSteps = location.SplitPathNamespaceSafe();

        var currentLocation = baseElement;
        foreach (var loc in locSteps)
            if (loc == ".")
            {
                // nothing to do
            }
            else if (loc == "..")
            {
                currentLocation = currentLocation.Parent;
                if (currentLocation == null)
                    break;
            }
            else
            {
                XName curLocName = loc;
                XElement? newLoc;
                if (curLocName.Namespace.IsEmpty())
                    newLoc = currentLocation.Element(curLocName);
                else
                    newLoc = currentLocation.Element_NamespaceNeutral(curLocName);

                if (newLoc == null)
                {
                    var newElem = new XElement(curLocName.OverrideNsIfEmpty(currentLocation.Name.Namespace));
                    currentLocation.Add(newElem);
                    currentLocation = newElem;
                }
                else
                {
                    currentLocation = newLoc;
                }
            }

        return currentLocation ?? baseElement;
    }

    /// <summary>
    /// Determines whether the attribute with the given name located in the given location string exists in the given XML
    /// element.
    /// </summary>
    /// <param name="baseElement">The XML element.</param>
    /// <param name="location">The location string.</param>
    /// <param name="attrName">Name of the attribute.</param>
    /// <returns>
    /// a value indicating whether the attribute with the given name located in the given location string exists in the
    /// given XML element.
    /// </returns>
    public static bool AttributeExists(XElement baseElement, string location, XName attrName)
    {
        return FindAttribute(baseElement, location, attrName) != null;
    }

    /// <summary>
    /// Finds the attribute with the given name located in the given location string in the given XML element.
    /// </summary>
    /// <param name="baseElement">The XML element.</param>
    /// <param name="location">The location string.</param>
    /// <param name="attrName">Name of the attribute.</param>
    /// <returns>
    /// a value indicating whether the attribute with the given name located in
    /// the given location string in the given XML element has been found.
    /// </returns>
    public static XAttribute? FindAttribute(XElement baseElement, string location, XName attrName)
    {
        var newLoc = FindLocation(baseElement, location);
        if (newLoc == null)
            return null;

        var newAttrName = attrName;
        // the following stupid code is because of odd behaviour of LINQ to XML
        if (newAttrName.Namespace == newLoc.Name.Namespace)
            newAttrName = newAttrName.RemoveNamespace();

        if (newAttrName.Namespace.IsEmpty())
            return newLoc.Attribute(newAttrName);
        return newLoc.Attribute_NamespaceNeutral(newAttrName);
    }

    /// <summary>
    /// Determines whether the attribute with the given name can be created in the location
    /// specified by the given location string in the given XML element.
    /// </summary>
    /// <param name="baseElement">The XML element.</param>
    /// <param name="location">The location string.</param>
    /// <param name="attrName">Name of the attribute.</param>
    /// <returns>
    /// <c>true</c> if the attribute with the given name can be created in the location
    /// specified by the given location string in the given XML element; otherwise, <c>false</c>.
    /// </returns>
    public static bool CanCreateAttribute(XElement baseElement, string location, XName attrName)
    {
        var newLoc = FindLocation(baseElement, location);
        if (newLoc == null) //if the location does not exist
        {
            if (CanCreateLocation(baseElement, location)) // see if you can create the location
                // if you can create the location you can create the attribute too
                return true;
            return false;
        }

        var newAttrName = attrName;
        // the following stupid code is because of odd behaviour of LINQ to XML
        if (newAttrName.Namespace == newLoc.Name.Namespace)
            newAttrName = newAttrName.RemoveNamespace();

        // check if the attribute does not exist
        if (newAttrName.Namespace.IsEmpty())
            return newLoc.Attribute(newAttrName) == null;
        return newLoc.Attribute_NamespaceNeutral(newAttrName) == null;
    }

    /// <summary>
    /// Creates and returns the attribute with the given name in the location
    /// specified by the given location string in the given XML element.
    /// </summary>
    /// <param name="baseElement">The XML element.</param>
    /// <param name="location">The location string.</param>
    /// <param name="attrName">Name of the attribute.</param>
    /// <param name="attrValue">The value to be assigned to the attribute.</param>
    /// <param name="documentDefaultNamespace">The default namespace.</param>
    /// <param name="culture">The <see cref="CultureInfo" /> to use for formatting the value.</param>
    /// <returns>
    /// returns the attribute with the given name in the location
    /// specified by the given location string in the given XML element.
    /// </returns>
    public static XAttribute? CreateAttribute(XElement baseElement, string location, XName attrName,
        object? attrValue, XNamespace documentDefaultNamespace, CultureInfo culture)
    {
        var newLoc = FindLocation(baseElement, location);
        if (newLoc == null)
        {
            if (CanCreateLocation(baseElement, location))
                newLoc = CreateLocation(baseElement, location);
            else
                return null;
        }

        // check if the attribute does not exist
        if (attrName.Namespace.IsEmpty() && attrName.Namespace != documentDefaultNamespace)
        {
            // i.e., the attribute already exists 
            if (newLoc.Attribute(attrName) != null)
                return null; // we cannot create another one with the same name
        }
        else
        {
            if (newLoc.Attribute_NamespaceNeutral(attrName) != null) // i.e., the attribute already exists
                return null; // we cannot create another one with the same name
        }

        return newLoc.AddAttributeNamespaceSafe(attrName, attrValue, documentDefaultNamespace, culture);
    }

    /// <summary>
    /// Finds the element with the given name located in the given location string in the given XML element.
    /// </summary>
    /// <param name="baseElement">The XML element.</param>
    /// <param name="location">The location string.</param>
    /// <param name="elemName">Name of the element.</param>
    /// <returns>
    /// a value indicating whether the element with the given name located in
    /// the given location string in the given XML element has been found
    /// </returns>
    public static XElement? FindElement(XElement baseElement, string location, XName elemName)
    {
        return FindLocation(baseElement, StringUtils.CombineLocationAndElementName(location, elemName));
    }

    /// <summary>
    /// Determines whether the XML element with the given name located in the
    /// given location string in the given XML element exists.
    /// </summary>
    /// <param name="baseElement">The XML element.</param>
    /// <param name="location">The location string.</param>
    /// <param name="elemName">Name of the element.</param>
    /// <returns>
    /// a value indicating whether the XML element with the given name located in the
    /// given location string in the given XML element exists
    /// </returns>
    public static bool ElementExists(XElement baseElement, string location, XName elemName)
    {
        return FindElement(baseElement, location, elemName) != null;
    }

    /// <summary>
    /// Determines whether the XML element with the given name located in the
    /// given location string in the given XML element can be created
    /// </summary>
    /// <param name="baseElement">The XML element.</param>
    /// <param name="location">The location string.</param>
    /// <param name="elemName">Name of the element.</param>
    /// <returns>
    /// <c>true</c> if the XML element with the given name located in the given
    /// location string in the given XML element can be created; otherwise, <c>false</c>.
    /// </returns>
    public static bool CanCreateElement(XElement baseElement, string location, XName elemName)
    {
        return CanCreateLocation(baseElement, StringUtils.CombineLocationAndElementName(location, elemName));
    }

    /// <summary>
    /// Creates and returns the XML element with the given name located in the
    /// given location string in the given XML element.
    /// </summary>
    /// <param name="baseElement">The parent XML element.</param>
    /// <param name="location">The location string.</param>
    /// <param name="elemName">Name of the element to create.</param>
    /// <returns>
    /// returns the XML element with the given name located in the
    /// given location string in the given XML element
    /// </returns>
    public static XElement CreateElement(XElement baseElement, string location, XName elemName)
    {
        return CreateLocation(baseElement, StringUtils.CombineLocationAndElementName(location, elemName));
    }

    /// <summary>
    /// Creates and returns the XML element with the given name located in the
    /// given location string in the given XML element.
    /// </summary>
    /// <param name="baseElement">The parent XML element.</param>
    /// <param name="location">The location string.</param>
    /// <param name="elemName">Name of the element to create.</param>
    /// <param name="elemValue">The element value to be assigned to the created element.</param>
    /// <returns>
    /// returns the XML element with the given name located in the
    /// given location string in the given XML element.
    /// </returns>
    public static XElement CreateElement(XElement baseElement, string location, XName elemName, object elemValue)
    {
        var elem = CreateElement(baseElement, location, elemName);
        elem.SetValue(elemValue);
        return elem;
    }

    /// <summary>
    /// Moves all the children of src (including all its elements and attributes) to the
    /// destination element, dst.
    /// </summary>
    /// <param name="src">The source element.</param>
    /// <param name="dst">The destination element.</param>
    public static void MoveDescendants(XElement src, XElement dst)
    {
        foreach (var attr in src.Attributes())
        {
            if (dst.Attribute(attr.Name) != null)
                throw new YAXAttributeAlreadyExistsException(attr.Name.ToString());

            dst.Add(attr);
        }

        foreach (var elem in src.Nodes()) dst.Add(elem);
    }

    /// <summary>
    /// Determines whether the specified element has neither any child attributes nor any child elements.
    /// </summary>
    /// <param name="elem">The element.</param>
    /// <returns>
    /// <c>true</c> if the specified element has neither any child attributes nor any child elements; otherwise,
    /// <c>false</c>.
    /// </returns>
    public static bool IsElementCompletelyEmpty(XElement? elem)
    {
        return elem != null && !elem.HasAttributes && !elem.HasElements && elem.IsEmpty;
    }

    /// <summary>
    /// Decodes the XML escape sequences into normal string
    /// </summary>
    /// <param name="str">The string to decode.</param>
    /// <returns></returns>
    public static string DecodeXMLString(string str)
    {
        if (str.Contains('&'))
            return str.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace("&apos;", "'")
                .Replace("&amp;", "&");
        // Make sure that &amp; is the final replace so that sequences such as &amp;gt; do not get corrupted

        return str;
    }

    /// <summary>
    /// Adds the 'xml:space="preserve"' attribute to the specified element.
    /// </summary>
    /// <param name="element">Element to add the 'xml:space="preserve"' attribute to</param>
    /// <param name="culture">The <see cref="CultureInfo" /> to use for string-formatting values.</param>
    /// <returns></returns>
    public static XElement AddPreserveSpaceAttribute(XElement element, CultureInfo culture)
    {
        element.AddAttributeNamespaceSafe(XNamespace.Xml + "space", "preserve", XNamespace.None, culture);
        return element;
    }

    public static string GetRandomPrefix(this XElement self)
    {
        var q = self.Attributes().Where(xa => xa.Name.Namespace == XNamespace.Xmlns).Select(xa => xa.Name.LocalName)
            .ToArray();
        var setPrefixes = new HashSet<string>(q);

        var prefix = "p";

        for (var i = 1; i <= 10000; i++)
            if (!setPrefixes.Contains(prefix + i))
                return prefix + i;

        throw new InvalidOperationException("Cannot create a unique random prefix");
    }

    /// <summary>
    /// Gets the string representation of the object, or <see cref="string.Empty" /> if the object is <see langword="null" />.
    /// </summary>
    /// <param name="self">The object to get as a string.</param>
    /// <param name="culture">The <see cref="CultureInfo" /> to use for culture-specific output.</param>
    /// <returns>
    /// The <see cref="CultureInfo" />-aware string representation of the object, or <see cref="string.Empty" /> if
    /// the object is <see langword="null" />.
    /// </returns>
    public static string ToXmlValue(this object? self, CultureInfo culture)
    {
        if (self == null) return string.Empty;

        return self.GetType().Name switch {
            "Boolean" => ((bool) self).ToString().ToLowerInvariant(),
            "Double" => ((double) self).ToString("R", culture),
            "Single" => ((float) self).ToString("R", culture),
            "BigInteger" => ReflectionUtils.InvokeMethod(self, "ToString", "R", culture) as string ?? string.Empty,
            _ => Convert.ToString(self, culture) ?? string.Empty
        };
    }

    public static XAttribute AddAttributeNamespaceSafe(this XElement parent, XName attrName, object? attrValue,
        XNamespace documentDefaultNamespace, CultureInfo culture)
    {
        var newAttrName = attrName;

        if (newAttrName.Namespace == documentDefaultNamespace)
            newAttrName = newAttrName.RemoveNamespace();

        var newAttr = new XAttribute(newAttrName, attrValue.ToXmlValue(culture));
        parent.Add(newAttr);
        return newAttr;
    }

    public static XAttribute? Attribute_NamespaceSafe(this XElement parent, XName attrName,
        XNamespace documentDefaultNamespace)
    {
        if (attrName.Namespace == documentDefaultNamespace)
            attrName = attrName.RemoveNamespace();
        return parent.Attribute(attrName);
    }

    public static IEnumerable<XAttribute> Attributes_NamespaceSafe(this XElement parent, XName attrName,
        XNamespace documentDefaultNamespace)
    {
        if (attrName.Namespace == documentDefaultNamespace)
            attrName = attrName.RemoveNamespace();
        return parent.Attributes(attrName);
    }

    /// <summary>
    /// Gets the XML content of an <see cref="XElement" /> with the value parameter formatted <see cref="CultureInfo" />
    /// -specific.
    /// </summary>
    /// <param name="self">The <see cref="XElement" /></param>
    /// <param name="contentValue">An <see cref="object" /> for the content value.</param>
    /// <param name="culture">The <see cref="CultureInfo" /> to use for string-formatting the content value.</param>
    /// <returns>
    /// The XML content of an <see cref="XElement" /> with the value parameter formatted <see cref="CultureInfo" />
    /// -specific.
    /// </returns>
    public static XElement AddXmlContent(this XElement self, object? contentValue, CultureInfo culture)
    {
        self.Add(new XText(contentValue.ToXmlValue(culture)));
        return self;
    }

    public static string GetXmlContent(this XElement self)
    {
        var values = self.Nodes().OfType<XText>().ToArray();
        if (values.Length > 0)
            return values[0].Value;

        return string.Empty;
    }

    public static XAttribute? Attribute_NamespaceNeutral(this XElement parent, XName name)
    {
        return parent.Attributes().FirstOrDefault(e => e.Name.LocalName == name.LocalName);
    }

    public static IEnumerable<XAttribute> Attributes_NamespaceNeutral(this XElement parent, XName name)
    {
        return parent.Attributes().Where(e => e.Name.LocalName == name.LocalName);
    }

    public static XElement? Element_NamespaceNeutral(this XContainer parent, XName name)
    {
        return parent.Elements().FirstOrDefault(e => e.Name.LocalName == name.LocalName);
    }

    public static IEnumerable<XElement> Elements_NamespaceNeutral(this XContainer parent, XName name)
    {
        return parent.Elements().Where(e => e.Name.LocalName == name.LocalName);
    }

    public static bool IsEmpty(this XNamespace self)
    {
        return !string.IsNullOrEmpty(self.NamespaceName.Trim());
    }

    public static XNamespace IfEmptyThen(this XNamespace self, XNamespace next)
    {
        return self.IsEmpty() ? self : next;
    }

    public static XNamespace IfEmptyThenNone(this XNamespace self)
    {
        return IfEmptyThen(self, XNamespace.None);
    }


    public static XName OverrideNsIfEmpty(this XName self, XNamespace ns)
    {
        if (self.Namespace.IsEmpty())
            return self;
        if (ns.IsEmpty())
            return ns + self.LocalName;
        return self;
    }

    public static XName RemoveNamespace(this XName self)
    {
        return XName.Get(self.LocalName);
    }
}
