// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;
using YAXLib;
using YAXLib.Customization;

namespace YAXLibTests.SampleClasses.CustomSerialization;

public class InterfaceSerializer : ICustomSerializer<ISampleInterface>
{
    public void SerializeToAttribute(ISampleInterface objectToSerialize, XAttribute attrToFill,
        ISerializationContext serializationContext)
    {
        throw new NotImplementedException();
    }

    public void SerializeToElement(ISampleInterface objectToSerialize, XElement elemToFill,
        ISerializationContext serializationContext)
    {
        // Serialize ISampleInterface members
        elemToFill.Add(new XElement("C_Id", objectToSerialize.Id));
        elemToFill.Add(new XElement("C_Name", objectToSerialize.Name));

        // Serialize another class member as comment
        // only when serializing GenericClassWithInterface
        var classType = serializationContext.TypeContext.Type!;
        if (classType.IsGenericType && classType.GetGenericTypeDefinition() == typeof(GenericClassWithInterface<>))
        {
            var valueOfT = objectToSerialize.GetType().GetProperty("Something")?.GetValue(objectToSerialize);
            elemToFill.Add(new XComment($"Value of 'Something': '{valueOfT}'"));
        }
    }

    public string SerializeToValue(ISampleInterface objectToSerialize, ISerializationContext serializationContext)
    {
        throw new NotImplementedException();
    }

    public ISampleInterface DeserializeFromAttribute(XAttribute attribute,
        ISerializationContext serializationContext)
    {
        throw new NotImplementedException();
    }

    public ISampleInterface DeserializeFromElement(XElement element, ISerializationContext serializationContext)
    {
        var o = (ISampleInterface?) Activator.CreateInstance(serializationContext.TypeContext.Type!);
        if (element.HasElements)
        {
            o!.Id = int.Parse(element.Element("C_" + nameof(ISampleInterface.Id))!.Value);
            o.Name = element.Element("C_" + nameof(ISampleInterface.Name))!.Value;
        }

        return o!;
    }

    public ISampleInterface DeserializeFromValue(string value, ISerializationContext serializationContext)
    {
        throw new NotImplementedException();
    }
}