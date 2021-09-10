// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;

namespace YAXLib.Helpers
{
    /// <summary>
    /// A helper that can instantiate a customer serializer.
    /// </summary>
    internal static class CustomSerializerActivatorHelper
    {
        internal static object InvokeCustomDeserializerFromElement(Type customDeserType, XElement elemToDeser)
        {
            var customDeserializer = Activator.CreateInstance(customDeserType, new object[0]);
            return customDeserType.InvokeMethod("DeserializeFromElement", customDeserializer,
                new object[] { elemToDeser });
        }

        internal static object InvokeCustomDeserializerFromAttribute(Type customDeserType, XAttribute attrToDeser)
        {
            var customDeserializer = Activator.CreateInstance(customDeserType, new object[0]);
            return customDeserType.InvokeMethod("DeserializeFromAttribute", customDeserializer,
                new object[] { attrToDeser });
        }

        internal static object InvokeCustomDeserializerFromValue(Type customDeserType, string valueToDeser)
        {
            var customDeserializer = Activator.CreateInstance(customDeserType, new object[0]);
            return customDeserType.InvokeMethod("DeserializeFromValue", customDeserializer,
                new object[] { valueToDeser });
        }

        internal static void InvokeCustomSerializerToElement(Type customSerType, object objToSerialize, XElement elemToFill)
        {
            var customSerializer = Activator.CreateInstance(customSerType, new object[0]);
            customSerType.InvokeMethod("SerializeToElement", customSerializer, new[] { objToSerialize, elemToFill });
        }

        internal static void InvokeCustomSerializerToAttribute(Type customSerType, object objToSerialize, XAttribute attrToFill)
        {
            var customSerializer = Activator.CreateInstance(customSerType, new object[0]);
            customSerType.InvokeMethod("SerializeToAttribute", customSerializer, new[] { objToSerialize, attrToFill });
        }

        internal static string InvokeCustomSerializerToValue(Type customSerType, object objToSerialize)
        {
            var customSerializer = Activator.CreateInstance(customSerType, new object[0]);
            return (string) customSerType.InvokeMethod("SerializeToValue", customSerializer, new[] { objToSerialize });
        }
    }
}
