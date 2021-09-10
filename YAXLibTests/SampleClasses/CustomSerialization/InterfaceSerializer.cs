// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;
using YAXLib;

namespace YAXLibTests.SampleClasses.CustomSerialization
{
    public class InterfaceSerializer : ICustomSerializer<ISampleInterface>
    {
        public void SerializeToAttribute(ISampleInterface objectToSerialize, XAttribute attrToFill)
        {
            throw new NotImplementedException();
        }

        public void SerializeToElement(ISampleInterface objectToSerialize, XElement elemToFill)
        {
            elemToFill.Add(new XElement("Id", objectToSerialize.Id));
            elemToFill.Add(new XElement("Name", objectToSerialize.Name));
        }

        public string SerializeToValue(ISampleInterface objectToSerialize)
        {
            throw new NotImplementedException();
        }

        public ISampleInterface DeserializeFromAttribute(XAttribute attrib)
        {
            throw new NotImplementedException();
        }

        public ISampleInterface DeserializeFromElement(XElement element)
        {
            throw new NotImplementedException();
        }

        public ISampleInterface DeserializeFromValue(string value)
        {
            throw new NotImplementedException();
        }
    }}