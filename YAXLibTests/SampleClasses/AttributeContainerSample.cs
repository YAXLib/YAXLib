using YAXLib;

namespace YAXLibTests.SampleClasses
{
    [YAXSerializeAs("container")]
    public class AttributeContainerSample
    {
        [YAXSerializeAs("range")]
        public AttributeSample Range { get; set; } 
    }

    public class AttributeSample
    {
        [YAXSerializeAs("from")]
        [YAXAttributeForClass]
        public int? From { get; set; }

        [YAXSerializeAs("to")]
        [YAXAttributeForClass]
        public int? To { get; set; }
    }

}