using YAXLib;

namespace YAXLibTests.SampleClasses
{
    [YAXSerializeAs("container")]
    public class AttributeContainerSample
    {
        [YAXSerializeAs("range")]
        public AttributeSample Range { get; set; } 
    }
}