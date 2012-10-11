using System;
using System.Collections.Generic;
using YAXLib;

namespace YAXLibTests.SampleClasses
{
    [ShowInDemoApplication(SortKey="_")]

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

        public static DictionaryContainerSample GetSampleInstance()
        {
            var container = new DictionaryContainerSample
            {
                Items =  DictionarySample.GetSampleInstance()
            };

            return container;
        }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }
    }

    [YAXSerializeAs("items")]
    [YAXNamespace("http://example.com/")]
    public class DictionarySample : Dictionary<string, string>
    {
        public static DictionarySample GetSampleInstance()
        {
            var dictionary = new DictionarySample
            {
                    { "key1", new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11).ToString()},
                    { "key2", 1234.ToString() },
            };

            return dictionary;
        }
    }

}