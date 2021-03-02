﻿using System;

namespace DemoApplication
{
    public class ClassInfoListItem
    {
        public Type ClassType { get; set; }
        public object SampleObject { get; set; }

        public ClassInfoListItem(Type classType, object sampleObject)
        {
            this.ClassType = classType;
            this.SampleObject = sampleObject;
        }

        public override string ToString()
        {
            return ClassType.Name;
        }
    }
}
