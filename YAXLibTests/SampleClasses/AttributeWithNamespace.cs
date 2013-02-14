using System;
using YAXLib;

namespace YAXLibTests
{
    [YAXSerializableType(Options = YAXSerializationOptions.DontSerializeNullObjects, FieldsToSerialize = YAXSerializationFields.AttributedFieldsOnly)]
    [YAXSerializeAs("font")]
    [YAXNamespace("w", "http://example.com/namespace")]
    public class AttributeWithNamespace
    {
        [YAXSerializableField]
        [YAXSerializeAs("{http://example.com/namespace}name")]
        [YAXAttributeForClass]
        public string Name {
            get;
            set;
        }

        public static AttributeWithNamespace GetSampleInstance()
        {
            return new AttributeWithNamespace() {
                Name = "Arial"
            };
        }
    }
}

