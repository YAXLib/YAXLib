#region License

// --------------------------------------------------
// Copyright © 2003-2011 OKB. All Rights Reserved.
// 
// This software is proprietary information of OKB.
// USE IS SUBJECT TO LICENSE TERMS.
// --------------------------------------------------

#endregion

using YAXLib;

namespace YAXLibTests.SampleClasses
{
    public class AttributeSample
    {
        [YAXSerializeAs("from")]
        [YAXAttributeForClass]
        public int? From { get; set; }

        [YAXSerializeAs("to")]
        [YAXAttributeForClass]
        public int? To { get; set; }
    }
}
