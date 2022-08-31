// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Xml.Linq;
using YAXLib;
using YAXLib.Customization;

namespace YAXLibTests.SampleClasses.CustomSerialization
{
    public class ClassLevelCtxSerializer : ICustomSerializer<ClassLevelCtxSample>
    {
        private const string Custom = " CUSTOM";

        public void SerializeToAttribute(ClassLevelCtxSample objectToSerialize, XAttribute attrToFill, ISerializationContext serializationContext)
        {
            // Note: Using ISerializationContext here is to complete unit test coverage
            var xElement = serializationContext.TypeContext.Serialize(objectToSerialize);
            attrToFill.Value = string.Join("|", "ATTR", xElement.Element(nameof(objectToSerialize.Title))!.Value,
                xElement.Element(nameof(objectToSerialize.MessageBody))!.Value);
        }

        public void SerializeToElement(ClassLevelCtxSample objectToSerialize, XElement elemToFill, ISerializationContext serializationContext)
        {
            // Note: Using ISerializationContext here is to complete unit test coverage
            var xElement = serializationContext.TypeContext.Serialize(objectToSerialize);
            var valTitle = xElement.Element(nameof(objectToSerialize.Title))!.Value  + Custom;
            xElement.Element(nameof(objectToSerialize.Title))!.Value = valTitle;
            var valMessageBody = xElement.Element(nameof(objectToSerialize.MessageBody))!.Value  + Custom;
            xElement.Element(nameof(objectToSerialize.MessageBody))!.Value = valMessageBody;

            elemToFill.Add(xElement.Element(nameof(objectToSerialize.Title)));
            elemToFill.Add(xElement.Element(nameof(objectToSerialize.MessageBody)));
        }

        public string SerializeToValue(ClassLevelCtxSample objectToSerialize, ISerializationContext serializationContext)
        {
            // Note: Using ISerializationContext here is to complete unit test coverage
            var xElement = serializationContext.TypeContext.Serialize(objectToSerialize);
            return string.Join("|", "VAL", xElement.Element(nameof(objectToSerialize.Title))!.Value,
                xElement.Element(nameof(objectToSerialize.MessageBody))!.Value);
        }

        public ClassLevelCtxSample DeserializeFromAttribute(XAttribute attribute, ISerializationContext serializationContext)
        {
            var split = attribute.Value.Split('|');
            var result = new ClassLevelCtxSample {Title = split[1], MessageBody = split[2]};

            // Note: Using ISerializationContext here is to complete unit test coverage
            return (ClassLevelCtxSample) serializationContext.TypeContext.Deserialize(
                serializationContext.TypeContext.Serialize(result));
        }

        public ClassLevelCtxSample DeserializeFromElement(XElement element, ISerializationContext serializationContext)
        {
            var result = (ClassLevelCtxSample) serializationContext.TypeContext.Deserialize(element);
            result.Title = result.Title.Replace(Custom, string.Empty);
            result.MessageBody = result.MessageBody.Replace(Custom, string.Empty);

            return result;
        }

        public ClassLevelCtxSample DeserializeFromValue(string value, ISerializationContext serializationContext)
        {
            var split = value.Split('|');
            var result = new ClassLevelCtxSample {Title = split[1], MessageBody = split[2]};

            // Note: Using ISerializationContext here is to complete unit test coverage
            return (ClassLevelCtxSample) serializationContext.TypeContext.Deserialize(
                serializationContext.TypeContext.Serialize(result));
        }
    }
}
