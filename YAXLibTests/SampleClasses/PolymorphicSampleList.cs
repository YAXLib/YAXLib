using System.Collections.Generic;

using YAXLib;

namespace YAXLibTests.SampleClasses
{
    [YAXSerializeAs("samples")]
    public class PolymorphicSampleList : List<PolymorphicSample>
    {
    }
}