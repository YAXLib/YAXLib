﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;
using YAXLib;

namespace YAXLibTests.SampleClasses.CustomSerialization
{
    public class ClassLevelSerializer : ICustomSerializer<ClassLevelSample>
    {
        public void SerializeToAttribute(ClassLevelSample objectToSerialize, XAttribute attrToFill, ISerializationContext serializationContext)
        {
            attrToFill.Value = string.Join("|", "ATTR", objectToSerialize.Title, objectToSerialize.MessageBody);
        }

        public void SerializeToElement(ClassLevelSample objectToSerialize, XElement elemToFill, ISerializationContext serializationContext)
        {
            elemToFill.Add(new XElement(nameof(objectToSerialize.Title), objectToSerialize.Title));
            elemToFill.Add(new XElement(nameof(objectToSerialize.MessageBody), objectToSerialize.MessageBody));
        }

        public string SerializeToValue(ClassLevelSample objectToSerialize, ISerializationContext serializationContext)
        {
            return string.Join("|", "VAL", objectToSerialize.Title, objectToSerialize.MessageBody);
        }

        public ClassLevelSample DeserializeFromAttribute(XAttribute attrib, ISerializationContext serializationContext)
        {
            var split = attrib.Value.Split('|');
            return new ClassLevelSample {Title = split[1], MessageBody = split[2]};
        }

        public ClassLevelSample DeserializeFromElement(XElement element, ISerializationContext serializationContext)
        {
            return new ClassLevelSample {
                Title = element.Element(nameof(ClassLevelSample.Title))?.Value,
                MessageBody = element.Element(nameof(ClassLevelSample.MessageBody))?.Value
            };
        }

        public ClassLevelSample DeserializeFromValue(string value, ISerializationContext serializationContext)
        {
            var split = value.Split('|');
            return new ClassLevelSample {Title = split[1], MessageBody = split[2]};

        }
    }
}