// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Xml.Linq;
using YAXLib;
using YAXLib.Customization;

namespace YAXLibTests.SampleClasses.CustomSerialization;

public class ClassLevelSerializer : ICustomSerializer<ClassLevelSample>
{
    private const string Custom = " CUSTOM";

    public void SerializeToAttribute(ClassLevelSample objectToSerialize, XAttribute attrToFill,
        ISerializationContext serializationContext)
    {
        attrToFill.Value = string.Join("|", "ATTR", objectToSerialize.Title, objectToSerialize.MessageBody);
    }

    public void SerializeToElement(ClassLevelSample objectToSerialize, XElement elemToFill,
        ISerializationContext serializationContext)
    {
        elemToFill.Add(new XElement(nameof(objectToSerialize.Title), objectToSerialize.Title + Custom));
        elemToFill.Add(new XElement(nameof(objectToSerialize.MessageBody), objectToSerialize.MessageBody + Custom));
    }

    public string SerializeToValue(ClassLevelSample objectToSerialize, ISerializationContext serializationContext)
    {
        return string.Join("|", "VAL", objectToSerialize.Title, objectToSerialize.MessageBody);
    }

    public ClassLevelSample DeserializeFromAttribute(XAttribute attribute,
        ISerializationContext serializationContext)
    {
        var split = attribute.Value.Split('|');
        return new ClassLevelSample { Title = split[1], MessageBody = split[2] };
    }

    public ClassLevelSample DeserializeFromElement(XElement element, ISerializationContext serializationContext)
    {
        return new ClassLevelSample {
            Title = element.Element(nameof(ClassLevelSample.Title))?
                .Value.Replace(Custom, string.Empty)!,
            MessageBody = element.Element(nameof(ClassLevelSample.MessageBody))?
                .Value.Replace(Custom, string.Empty)!
        };
    }

    public ClassLevelSample DeserializeFromValue(string value, ISerializationContext serializationContext)
    {
        var split = value.Split('|');
        return new ClassLevelSample { Title = split[1], MessageBody = split[2] };
    }
}