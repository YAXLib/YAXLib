﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
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
    }

    public class ClassLevelSampleAsElement
    {
        public ClassLevelSample ClassLevelSample { get; set; }
        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }
    }
    
    public class ClassLevelSampleAsAttribute
    {
        [YAXAttributeForClass]
        public ClassLevelSample ClassLevelSample { get; set; }
        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }
    }
    
    public class ClassLevelSampleAsValue
    {
        [YAXValueForClass]
        public ClassLevelSample ClassLevelSample { get; set; }
        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }
    }
}
