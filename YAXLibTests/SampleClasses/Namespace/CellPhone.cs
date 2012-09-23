using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YAXLib;

namespace YAXLibTests.SampleClasses.Namespace
{
    [YAXNamespace("path/to/some/namespace")]
    public class CellPhone
    {
        public string DeviceBrand { get; set; }
        public string OS { get; set; }

        public static CellPhone GetSampelInstance()
        {
            return new CellPhone 
            { 
                DeviceBrand = "HTC",
                OS = "Windows Phone 8"
            };
        }
    }
}
