namespace YAXLibTests.SampleClasses.SelfReferencingObjects
{
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
            // this must be serialized fine, because there's no loop, although the type is a self referring type.
            // However by setting, "second.Next = first;" It should not be serialized any more because it will cause a loop
            
            return first;
        }

        public static SelfReferringTypeIsNotASelfReferringObject GetSampleInstanceWithLoop()
        {
            var instance = GetSampleInstance();
            instance.Next.Next = instance;
            return instance;
        }
    }
}
