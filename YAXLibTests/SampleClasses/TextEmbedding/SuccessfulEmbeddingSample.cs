// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

#nullable enable
using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses.TextEmbedding;

internal class SuccessfulEmbeddingSample
{
    [YAXTextEmbedding(YAXLib.Enums.TextEmbedding.None)]
    [YAXSerializeAs("Text-No-Embedding")]
    public string TextNoEmbedding { get; set; } = string.Empty;

    [YAXTextEmbedding(YAXLib.Enums.TextEmbedding.CData)]
    public string TextCDataEmbedding { get; set; } = string.Empty;

    [YAXTextEmbedding(YAXLib.Enums.TextEmbedding.Base64)]
    public string TextBase64Embedding { get; set; } = string.Empty;

    [YAXTextEmbedding(YAXLib.Enums.TextEmbedding.Base64)]
    public string? TextIsNull { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static SuccessfulEmbeddingSample GetSampleInstance()
    {
        return new SuccessfulEmbeddingSample {
            TextNoEmbedding = "< Text & NoEmbedding >",
            // A compliant XML parsers must, before parsing,
            // translate CRLF and any CR not followed by a LF to a single LF.
            // This behavior is defined in the End-of-Line handling section of the XML 1.0 specification.
            // So, for the unit tests we have to replace CRLF with LF for consistent comparisons.
            TextCDataEmbedding = """
                <script>
                    let X = 4; let Y = 5; let Z = 8;
                    if (Y < Z && Y > X) {
                        console.log(`'X < Y < Z' or 'Z > Y > X'`);
                    }
                </script>
                """.Replace("\r\n", "\n"),
            // All chars will be encoded
            TextBase64Embedding = "part1\u0000part2"
        };
    }
}