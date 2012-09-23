using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YAXLib;

namespace YAXLibTests.SampleClasses.Namespace
{
    [ShowInDemoApplication(SortKey="_")]

    [YAXNamespace("path/to/some/namespace")]
    public class CellPhone
    {
        [YAXSerializeAs("Level1/Level2")]
        public string DeviceBrand { get; set; }
        public string OS { get; set; }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }

        public static CellPhone GetSampleInstance()
        {
            return new CellPhone 
            { 
                DeviceBrand = "HTC",
                OS = "Windows Phone 8"
            };
        }
    }
}
