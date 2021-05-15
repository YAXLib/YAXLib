// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;
using YAXLib.Options;

namespace YAXLib
{
    internal class ExceptionKnownBaseType : KnownType<Exception>
    {
        internal const string ObjectGraphElementName = "Exception-Graph";
        private int _recursion;

        public override bool CanSerialize => true;
        public override bool CanDeserialize => true;
        internal bool SerializeObjectGraph { get; set; } = true;

        public override void Serialize(Exception obj, XElement elem, XNamespace overridingNamespace)
        {
            if (obj == null) return;

            if (_recursion >= Options.MaxRecursion)
                return;

            // We serialize the whole object graph
            if (_recursion == 0)
            {
                elem.Name = nameof(Exception);
                // Unprefixed attributes use the default namespace, i.e. mostly from the element
                elem.Add(new XAttribute("type", obj.GetType().AssemblyQualifiedName ?? string.Empty));

                if (SerializeObjectGraph && TrySerializeObjectGraph(obj, out var value))
                {
                    var objElement = new XElement(ObjectGraphElementName, overridingNamespace, value);
                    elem.Add(new XComment("Base64 encoded result from " + typeof(System.Runtime.Serialization.Formatters.Binary.BinaryFormatter).FullName));
                    elem.Add(objElement);
                }
            }

            foreach (var member in new YAXSerializer(obj.GetType(), new SerializerOptions{MaxRecursion = 1}).GetFieldsToBeSerialized())
            {
                // We MUST NOT use a custom serializer for Exception types, as this known type class is expected
                // to process this type. Doing different will lead to an infinite loop.
                if (!ReflectionUtils.IsBaseClassOrSubclassOf(member.MemberType, "System.Exception"))
                {
                    var value = member.GetValue(obj);
                    if (value == null || ReflectionUtils.IsBasicType(value.GetType()))
                    {
                        elem.Add(new XElement(member.Alias, overridingNamespace, value));
                    }
                    else if (member.OriginalName == nameof(Exception.TargetSite))
                    {
                        elem.Add(new XElement(member.Alias, overridingNamespace, value));
                    }
                    else
                    {
                        // Without a recursion limit it could happen
                        // that this is instance gets called from the memberSerializer
                        var memberSerializer = new YAXSerializer(member.MemberType, new SerializerOptions {MaxRecursion = 1});
                        var parent = new XElement(member.Alias, overridingNamespace);
                        var element = memberSerializer.SerializeToXDocument(value).Root;
                        if (!XMLUtils.IsElementCompletelyEmpty(element)) parent.Add(element);
                        elem.Add(parent);
                    }
                } 
                else
                {
                    var exElement =
                        new XElement(member.OriginalName == "InnerException" ? member.OriginalName : "Exception",
                            overridingNamespace); // Alias not used
                    var ex = member.GetValue(obj) as Exception;
                    if (ex != null)
                    {
                        exElement.Add(new XAttribute("type", ex.GetType().FullName ?? string.Empty)); // Alias not used
                    }
                    elem.Add(exElement);
                    _recursion++;
                    Serialize(ex, exElement, overridingNamespace);
                    _recursion--;
                }
            }
        }

        public override Exception? Deserialize(XElement elem, XNamespace overridingNamespace)
        {
            // Only the Exception can be deserialized
            if (SerializeObjectGraph && TryDeserializeObjectGraph(elem, out var exception))
            {
                return exception;
            }

            var exType = Type.GetType(elem.Attribute("type")?.Value ?? string.Empty);
            if (exType != null)
            {
                var exInstance = Activator.CreateInstance(exType, Array.Empty<object>()) as Exception;
                return exInstance;
            }

            return null;
        }

        private static bool TrySerializeObjectGraph(object obj, out string? value)
        {
            try
            {
                using var stream = new System.IO.MemoryStream();
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Flush();
                value = Convert.ToBase64String(stream.GetBuffer(), 0, (int) stream.Length);
                return true;
            }
            catch
            {
                // Not marked as Serializable
                // Nothing to do
            }
            
            value = null;
            return false;
        }

        private static bool TryDeserializeObjectGraph(XElement elem, out Exception? exception)
        {
            try
            {
                var oGraph = XMLUtils.FindElement(elem, ".", ObjectGraphElementName);
                if (oGraph != null)
                {
                    using var stream = new System.IO.MemoryStream(Convert.FromBase64String(oGraph.Value));
                    stream.Seek(0, System.IO.SeekOrigin.Begin);
                    var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    exception = formatter.Deserialize(stream) as Exception;
                    stream.Close();
                    return true;
                }
            }
            catch
            {
                // Nothing to do
            }

            exception = null;
            return false;
        }
    }
}