using System.Collections.Generic;
using YAXLib;

namespace YAXLibTests.SampleClasses.Namespace
{
    public class CellPhone_DictionaryNamespaceForAllItems
    {
        [YAXSerializeAs("{http://namespace.org/brand}Brand")]
        public string DeviceBrand { get; set; }

        public string OS { get; set; }

        [YAXSerializeAs("{http://namespace.org/prices}ThePrices")]
        [YAXDictionary(EachPairName="{http://namespace.org/pricepair}PricePair",
            KeyName="{http://namespace.org/color}TheColor", 
            ValueName="{http://namespace.org/pricevalue}ThePrice")]
        public Dictionary<string, double> Prices { get; set; }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }

        public static CellPhone_DictionaryNamespaceForAllItems GetSampleInstance()
        {
            var prices = new Dictionary<string, double> { { "red", 120 }, { "blue", 110 }, { "black", 140 } };
            return new CellPhone_DictionaryNamespaceForAllItems 
            { 
                DeviceBrand = "Samsung Galaxy Nexus",
                OS = "Android",
                Prices = prices,
            };
        }
    }
}
