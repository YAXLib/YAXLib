using System.Collections.Generic;

using YAXLib;

namespace YAXLibTests.SampleClasses
{
    [YAXSerializeAs("items")]
    [YAXNamespace("http://example.com/")]
    public class DictionarySample : Dictionary<string, object>
    {
    }
}