// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;
using YAXLib;

namespace YAXLibTests.SampleClasses.CustomSerialization
{
    public class ClassLevelSerializer : ICustomSerializer<ClassLevelSample>
    {
        public void SerializeToAttribute(ClassLevelSample objectToSerialize, XAttribute attrToFill)
        {
            throw new NotImplementedException();
        }

        public void SerializeToElement(ClassLevelSample objectToSerialize, XElement elemToFill)
        {
            elemToFill.Add(new XText(objectToSerialize.MessageBody));
            elemToFill.SetAttributeValue(nameof(objectToSerialize.Title), objectToSerialize.Title);
        }

        public string SerializeToValue(ClassLevelSample objectToSerialize)
        {
            throw new NotImplementedException();
        }

        public ClassLevelSample DeserializeFromAttribute(XAttribute attrib)
        {
            throw new NotImplementedException();
        }

        public ClassLevelSample DeserializeFromElement(XElement element)
        {
            return new ClassLevelSample
                {MessageBody = element.Value, Title = element.Attribute(nameof(ClassLevelSample.Title))?.Value};
        }

        public ClassLevelSample DeserializeFromValue(string value)
        {
            throw new NotImplementedException();
        }
    }
}