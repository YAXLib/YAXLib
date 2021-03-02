using YAXLib;

namespace YAXLibTests.SampleClasses.PolymorphicSerialization
{
    public class MultipleYaxTypeAttributesWithSameAlias
    {
        [YAXType(typeof(int), Alias = "SameAlias")]
        [YAXType(typeof(string), Alias = "SameAlias")]
        public object Object { get; set; }

    }
}
