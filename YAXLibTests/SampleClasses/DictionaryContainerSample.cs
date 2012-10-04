using YAXLib;

namespace YAXLibTests.SampleClasses
{
    [YAXSerializeAs("container")]
    [YAXNamespace("http://example.com")]
    public class DictionaryContainerSample
    {
        [YAXSerializeAs("items")]
        [YAXDictionary(EachPairName = "item",
            KeyName = "key",
            SerializeKeyAs = YAXNodeTypes.Attribute
            /* TODO: Add YAXNodeTypes.Content so the value can be the the content of the <item/> element.
             * SerializeValueAs = YAXNodeTypes.Content */
            )]
        public DictionarySample Items { get; set; }
    }
}