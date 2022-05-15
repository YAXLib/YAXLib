﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;
using YAXLib;

namespace YAXLibTests.SampleClasses.CustomSerialization
{
    public class FieldLevelCombinedSerializer : ICustomSerializer<string>
    {
        public void SerializeToAttribute(string objectToSerialize, XAttribute attrToFill, ISerializationContext serializationContext)
        {
            attrToFill.Value = "ATTR_" + objectToSerialize;
        }

        public void SerializeToElement(string objectToSerialize, XElement elemToFill, ISerializationContext serializationContext)
        {
            elemToFill.Add(new XText("ELE__" + objectToSerialize));
        }

        public string SerializeToValue(string objectToSerialize, ISerializationContext serializationContext)
        {
            return "VAL__" + objectToSerialize;
        }

        public string DeserializeFromAttribute(XAttribute attrib, ISerializationContext serializationContext)
        {
            return attrib.Value.Substring(5);
        }

        public string DeserializeFromElement(XElement element, ISerializationContext serializationContext)
        {
            return element.Value.Substring(5);
        }

        public string DeserializeFromValue(string value, ISerializationContext serializationContext)
        {
            return value.Substring(5);
        }
    }
}