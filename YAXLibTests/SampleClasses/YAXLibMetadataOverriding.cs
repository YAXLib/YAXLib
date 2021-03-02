using YAXLib;

namespace YAXLibTests.SampleClasses
{
    [ShowInDemoApplication]
    public class YAXLibMetadataOverriding
    {
        public int[,] IntArray { get; set; }

        public object Obj { get; set; }


        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }

        public static YAXLibMetadataOverriding GetSampleInstance()
        {
            YAXLibMetadataOverriding instance = new YAXLibMetadataOverriding();
            instance.SetSampleData();
            return instance;
        }

        protected void SetSampleData()
        {
            IntArray = new int[2, 3];

            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 3; j++)
                    IntArray[i, j] = i + j + 1;

            Obj = "Hello, World!";
        }
    }

    [ShowInDemoApplication]
    [YAXNamespace("http://namespace.org/sample")]
    public class YAXLibMetadataOverridingWithNamespace : YAXLibMetadataOverriding
    {
        public static new YAXLibMetadataOverridingWithNamespace GetSampleInstance()
        {
            YAXLibMetadataOverridingWithNamespace instance = new YAXLibMetadataOverridingWithNamespace();
            instance.SetSampleData();
            return instance;
        }
    }
}
