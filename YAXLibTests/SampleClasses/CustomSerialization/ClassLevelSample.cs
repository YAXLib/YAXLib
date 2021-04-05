// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAXLib;

namespace YAXLibTests.SampleClasses.CustomSerialization
{
    [YAXCustomSerializer(typeof(ClassLevelSerializer))]
    public class ClassLevelSample
    {
        public string MessageBody { get; set; }
        
        public string Title { get; set; }
        
        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }
    }
}
