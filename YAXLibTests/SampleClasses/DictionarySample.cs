using System.Collections.Generic;

using YAXLib;

namespace YAXLibTests.SampleClasses
{
    [YAXSerializeAs("items")]
    public class DictionarySample : Dictionary<string, string>
    {
    }
}