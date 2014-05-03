namespace YAXLibTests.SampleClasses.SelfReferencingObjects
{
    [ShowInDemoApplication(SortKey = "_")]
    public class SelfReferringTypeIsNotASelfReferringObject
    {
        public int Data { get; set; }
        public SelfReferringTypeIsNotASelfReferringObject Next { get; set; }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }

        public static SelfReferringTypeIsNotASelfReferringObject GetSampleInstance()
        {
            var first = new SelfReferringTypeIsNotASelfReferringObject {Data = 1};
            var second = new SelfReferringTypeIsNotASelfReferringObject {Data = 2};
            first.Next = second;

            return first;
        }
    }
}
