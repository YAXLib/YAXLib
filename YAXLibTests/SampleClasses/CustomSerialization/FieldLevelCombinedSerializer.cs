// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;
using YAXLib;

namespace YAXLibTests.SampleClasses.CustomSerialization
{
    public class FieldLevelCombinedSerializer : ICustomSerializer<string>
    {
        public void SerializeToAttribute(string objectToSerialize, XAttribute attrToFill)
        {
            attrToFill.Value = "ATTR_" + objectToSerialize;
        }

        public void SerializeToElement(string objectToSerialize, XElement elemToFill)
        {
            elemToFill.Add(new XText("ELE__" + objectToSerialize));
        }

        public string SerializeToValue(string objectToSerialize)
        {
            return "VAL__" + objectToSerialize;
        }

        public string DeserializeFromAttribute(XAttribute attrib)
        {
            return attrib.Value.Substring(5);
        }

        public string DeserializeFromElement(XElement element)
        {
            return element.Value.Substring(5);
        }

        public string DeserializeFromValue(string value)
        {
            return value.Substring(5);
        }
    }
}