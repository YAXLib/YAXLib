// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication]
public class DictionaryKeyValueAsContent
{
    [YAXDictionary(EachPairName = "Pair", KeyName = "Digits", ValueName = "Letters",
        SerializeKeyAs = YAXNodeTypes.Attribute, SerializeValueAs = YAXNodeTypes.Content)]
    public Dictionary<int, string> DicValueAsContent { get; set; } = new();

    [YAXDictionary(EachPairName = "Pair", KeyName = "Digits", ValueName = "Letters",
        SerializeKeyAs = YAXNodeTypes.Content, SerializeValueAs = YAXNodeTypes.Attribute)]
    public Dictionary<int, string> DicKeyAsContnet { get; set; } = new();

    [YAXDictionary(EachPairName = "Pair", KeyName = "Digits", ValueName = "Letters",
        SerializeKeyAs = YAXNodeTypes.Content, SerializeValueAs = YAXNodeTypes.Element)]
    public Dictionary<int, string> DicKeyAsContentValueAsElement { get; set; } = new();

    [YAXDictionary(EachPairName = "Pair", KeyName = "Digits", ValueName = "Letters",
        SerializeKeyAs = YAXNodeTypes.Element, SerializeValueAs = YAXNodeTypes.Content)]
    public Dictionary<int, string> DicValueAsContentKeyAsElement { get; set; } = new();

    public static DictionaryKeyValueAsContent GetSampleInstance()
    {
        var dic = new Dictionary<int, string> { { 1, "one" }, { 2, "two" }, { 3, "three" } };

        return new DictionaryKeyValueAsContent {
            DicValueAsContent = dic,
            DicKeyAsContnet = dic,
            DicKeyAsContentValueAsElement = dic,
            DicValueAsContentKeyAsElement = dic
        };
    }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }
}