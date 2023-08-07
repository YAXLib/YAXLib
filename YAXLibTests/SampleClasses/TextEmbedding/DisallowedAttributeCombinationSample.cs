// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

#nullable enable
using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses.TextEmbedding;

internal class DisallowedAttributeCombinationSample
{
    [YAXAttributeFor(".")]
    [YAXTextEmbedding(YAXLib.Enums.TextEmbedding.CData)]
    public string TextCDataEmbedding { get; set; } = string.Empty;

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static DisallowedAttributeCombinationSample GetSampleInstance()
    {
        return new DisallowedAttributeCombinationSample {
            TextCDataEmbedding = """
                <script>
                    let X = 4; let Y = 5; let Z = 8;
                    if (Y < Z && Y > X) {
                        console.log(`'X < Y < Z' or 'Z > Y > X'`);
                    }
                </script>
                """.Replace("\r\n", "\n")
        };
    }
}