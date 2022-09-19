// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Text;
using System.Xml.Linq;
using YAXLib;
using YAXLib.Attributes;
using YAXLib.Customization;

namespace YAXLibTests.SampleClasses;

[YAXCustomSerializer(typeof(CustomMessageClassSerializer))]
public class Message
{
    public string? MessageText { get; set; }
    public string? BoldContent { get; set; }
    public int BoldIndex { get; set; }
    public int BoldLength { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }
}

[ShowInDemoApplication]
public class CustomSerializationDemoClasses
{
    [YAXCustomSerializer(typeof(CustomTitleSerializer))]
    [YAXElementFor("SomeTitle")]
    public string? Title { get; set; }

    public Message Message { get; set; } = new();

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static CustomSerializationDemoClasses GetSampleInstance()
    {
        var message = new Message {
            MessageText = "The sun is shining",
            BoldContent = "sun",
            BoldIndex = 4,
            BoldLength = 3
        };

        return new CustomSerializationDemoClasses {
            Title = "Important Note",
            Message = message
        };
    }
}

public class CustomMessageClassSerializer : ICustomSerializer<Message>
{
    public void SerializeToAttribute(Message objectToSerialize, XAttribute attrToFill,
        ISerializationContext serializationContext)
    {
        throw new NotImplementedException();
    }

    public void SerializeToElement(Message objectToSerialize, XElement elemToFill,
        ISerializationContext serializationContext)
    {
        var message = objectToSerialize.MessageText ?? string.Empty;
        var beforeBold = message.Substring(0, objectToSerialize.BoldIndex);
        var afterBold = message.Substring(objectToSerialize.BoldIndex + objectToSerialize.BoldLength);

        elemToFill.Add(new XText(beforeBold));
        elemToFill.Add(new XElement("b", objectToSerialize.BoldContent));
        elemToFill.Add(new XText(afterBold));
    }

    public string SerializeToValue(Message objectToSerialize, ISerializationContext serializationContext)
    {
        throw new NotImplementedException();
    }

    public Message DeserializeFromAttribute(XAttribute attribute, ISerializationContext serializationContext)
    {
        throw new NotImplementedException();
    }

    public Message DeserializeFromElement(XElement element, ISerializationContext serializationContext)
    {
        var wholeMessage = "";
        var boldIndex = -1;
        var boldLength = -1;
        var boldContent = "";

        foreach (var node in element.Nodes())
            if (node is XText)
            {
                wholeMessage += ((node as XText)!).Value;
            }
            else if (node is XElement)
            {
                var boldElement = node as XElement;
                if (boldElement?.Name.ToString() == "b")
                {
                    boldContent = boldElement.Value;
                    boldIndex = wholeMessage.Length;
                    boldLength = boldContent.Length;
                    wholeMessage += boldContent;
                }
            }

        return new Message {
            MessageText = wholeMessage,
            BoldContent = boldContent,
            BoldIndex = boldIndex,
            BoldLength = boldLength
        };
    }

    public Message DeserializeFromValue(string value, ISerializationContext serializationContext)
    {
        throw new NotImplementedException();
    }
}

public class CustomTitleSerializer : ICustomSerializer<string>
{
    public string DeserializeFromAttribute(XAttribute attribute, ISerializationContext serializationContext)
    {
        return RetrieveValue(attribute.Value);
    }

    public string DeserializeFromElement(XElement element, ISerializationContext serializationContext)
    {
        return RetrieveValue(element.Value);
    }

    public string DeserializeFromValue(string value, ISerializationContext serializationContext)
    {
        return RetrieveValue(value);
    }

    private static string RetrieveValue(string str)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < str.Length; i++)
            if (i % 2 == 0)
                sb.Append(str[i]);

        return sb.ToString();
    }

    public void SerializeToAttribute(string objectToSerialize, XAttribute attrToFill,
        ISerializationContext serializationContext)
    {
        attrToFill.Value = CreateMixedValue(objectToSerialize);
    }

    public void SerializeToElement(string objectToSerialize, XElement elemToFill,
        ISerializationContext serializationContext)
    {
        elemToFill.Value = CreateMixedValue(objectToSerialize);
    }

    public string SerializeToValue(string objectToSerialize, ISerializationContext serializationContext)
    {
        return CreateMixedValue(objectToSerialize);
    }

    private static string CreateMixedValue(string str)
    {
        var sb = new StringBuilder(2 * str.Length);

        for (var i = 0; i < str.Length; i++) sb.Append(str[i] + ".");

        return sb.ToString();
    }
}