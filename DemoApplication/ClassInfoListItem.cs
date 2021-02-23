// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
