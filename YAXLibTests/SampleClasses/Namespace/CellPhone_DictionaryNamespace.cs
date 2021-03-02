using System.Collections.Generic;
using YAXLib;

namespace YAXLibTests.SampleClasses.Namespace
{

    [YAXNamespace("http://namespace.org/nsmain")]
    public class CellPhone_DictionaryNamespace
    {
        [YAXSerializeAs("TheName")]
        [YAXNamespace("x1", "http://namespace.org/x1")]
        public string DeviceBrand { get; set; }

        public string OS { get; set; }

        [YAXNamespace("p1", "namespace/for/prices/only")]
        public Dictionary<string, double> Prices { get; set; }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }

        public static CellPhone_DictionaryNamespace GetSampleInstance()
        {
            var prices = new Dictionary<string, double> { { "red", 120 }, { "blue", 110 }, { "black", 140 } };

            return new CellPhone_DictionaryNamespace
            {
                DeviceBrand = "HTC",
                OS = "Windows Phone 8",
                Prices = prices
            };
        }
    }
}
