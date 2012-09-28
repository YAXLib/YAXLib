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
    public class CellPhone
    {
        [YAXElementFor("Level1/Level2")]
        [YAXSerializeAs("TheName")]
        [YAXNamespace("x1", "another/name/space/hah")]
        public string DeviceBrand { get; set; }
        public string OS { get; set; }

        [YAXNamespace("p1", "namespace/for/prices/only")]
        public Dictionary<Color, double> Prices { get; set; }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }

        public static CellPhone GetSampleInstance()
        {
            var prices = new Dictionary<Color, double> { { Color.Red, 120 }, { Color.Blue, 110 }, { Color.Black, 140 } };
            return new CellPhone 
            { 
                DeviceBrand = "HTC",
                OS = "Windows Phone 8",
                Prices = prices
            };
        }
    }
}
