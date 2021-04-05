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
            attrToFill.Value = string.Join("|", "ATTR", objectToSerialize.Title, objectToSerialize.MessageBody);
        }

        public void SerializeToElement(ClassLevelSample objectToSerialize, XElement elemToFill)
        {
            elemToFill.Add(new XElement(nameof(objectToSerialize.Title), objectToSerialize.Title));
            elemToFill.Add(new XElement(nameof(objectToSerialize.MessageBody), objectToSerialize.MessageBody));
        }

        public string SerializeToValue(ClassLevelSample objectToSerialize)
        {
            return string.Join("|", "VAL", objectToSerialize.Title, objectToSerialize.MessageBody);
        }

        public ClassLevelSample DeserializeFromAttribute(XAttribute attrib)
        {
            var split = attrib.Value.Split('|');
            return new ClassLevelSample {Title = split[1], MessageBody = split[2]};
        }

        public ClassLevelSample DeserializeFromElement(XElement element)
        {
            return new ClassLevelSample {
                Title = element.Element(nameof(ClassLevelSample.Title))?.Value,
                MessageBody = element.Element(nameof(ClassLevelSample.MessageBody))?.Value
            };
        }

        public ClassLevelSample DeserializeFromValue(string value)
        {
            var split = value.Split('|');
            return new ClassLevelSample {Title = split[1], MessageBody = split[2]};

        }
    }
}