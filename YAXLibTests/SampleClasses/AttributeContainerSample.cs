using YAXLib;

namespace YAXLibTests.SampleClasses
{
    [YAXSerializeAs("container")]
    public class AttributeContainerSample
    {
        [YAXSerializeAs("range")]
        public AttributeSample Range { get; set; } 

        public static AttributeContainerSample GetSampleInstance()
        {
            var container = new AttributeContainerSample
            {
                Range = new AttributeSample
                {
                    From = 1,
                    To = 3,
                }
            };

            return container;
        }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }
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