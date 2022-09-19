// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication]
[YAXComment("This example is used in the article to show YAXLib exception handling policies")]
public class ProgrammingLanguage
{
    [YAXErrorIfMissed(YAXExceptionTypes.Error)]
    public string? LanguageName { get; set; }

    [YAXErrorIfMissed(YAXExceptionTypes.Warning, DefaultValue = true)]
    public bool IsCaseSensitive { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static ProgrammingLanguage GetSampleInstance()
    {
        return new ProgrammingLanguage {
            LanguageName = "C#",
            IsCaseSensitive = true
        };
    }
}