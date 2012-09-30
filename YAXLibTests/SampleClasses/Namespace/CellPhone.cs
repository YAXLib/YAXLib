using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using YAXLib;

namespace YAXLibTests.SampleClasses.Namespace
{
    [ShowInDemoApplication(SortKey="_")]

    [YAXNamespace("path/to/some/namespace")]
    [YAXSerializeAs("MobilePhone")]
    public class CellPhone
    {
        [YAXAttributeFor("Sth")]
        public Color ItsColor { get; set; }

        [YAXElementFor("Level1/Level2")]
        [YAXSerializeAs("{Some/Other/Namespace}TheName")]
        //[YAXNamespace("x1", "another/name/space/hah")]
        public string DeviceBrand { get; set; }

        public string OS { get; set; }

        [YAXSerializeAs("{aasdasd/asdasd/asdsd}ThePs")]
        //[YAXNamespace("pr1", "namespace/for/prices/only")]
        [YAXDictionary(EachPairName = "{yet/another/one}PairItem", 
            KeyName="{ns/for/key}TheColor", ValueName="{ns/for/value}ThePrice")]
        public Dictionary<Color, double> Prices { get; set; }

        public InstalledApplications InstalledApplications { get; set; }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }

        public static CellPhone GetSampleInstance()
        {
            var prices = new Dictionary<Color, double> { { Color.Red, 120 }, { Color.Blue, 110 }, { Color.Black, 140 } };
            var installedApplications = new InstalledApplications();
            installedApplications.ListInstalledApplications = new List<InstalledApplication>()
            {
                new InstalledApplication() { Name = "Internet Explorer", Version = "10.0" },
                new InstalledApplication() { Name = "Facebook", Version = "5.0" }
            };
                
                //.Add(new InstalledApplication() { Name
            return new CellPhone 
            { 
                ItsColor = Color.Red,
                DeviceBrand = "HTC",
                OS = "Windows Phone 8",
                Prices = prices,
                InstalledApplications = installedApplications
            };
        }
    }

    public class InstalledApplications
    {
        public List<InstalledApplication> ListInstalledApplications { get; set; }
    }

    public class InstalledApplication
    {
        [YAXNamespace("ins", "some/inner/namespace/for/apps")]
        public string Name { get; set; }
        public string Version { get; set; }
    }
}
