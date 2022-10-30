// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

namespace YAXLibTests.SampleClasses.SelfReferencingObjects;

public class CalculatedPropertiesCanCauseInfiniteLoop
{
    public decimal Data { get; set; }

    public CalculatedPropertiesCanCauseInfiniteLoop? Reciprocal
    {
        get
        {
            if (Data == 0M)
                return null;

            var reciprocal = 1.0M / Data;
            return new CalculatedPropertiesCanCauseInfiniteLoop { Data = reciprocal };
        }
    }

    public static CalculatedPropertiesCanCauseInfiniteLoop GetSampleInstance()
    {
        return new CalculatedPropertiesCanCauseInfiniteLoop { Data = 2.0M };
    }

    public override string ToString()
    {
        return $"Data == {Data}";
    }
}