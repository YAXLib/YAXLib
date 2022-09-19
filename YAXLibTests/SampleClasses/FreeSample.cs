// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

namespace YAXLibTests.SampleClasses;

public class FreeSample
{
    public int BoundViewId { get; set; }
    public decimal SomeDecimalNumber { get; set; }

    public static FreeSample GetSampleInstance()
    {
        return new FreeSample {
            BoundViewId = 17,
            SomeDecimalNumber = 12948923849238402394
        };
    }


    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }
}