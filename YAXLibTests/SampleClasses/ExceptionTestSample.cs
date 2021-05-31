// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Threading;

namespace YAXLibTests.SampleClasses
{
    public class CustomException : Exception
    {
        public CustomException(string message) : this()
        {
            Info = message;
        }
        public CustomException()
        {
            Info = "Parameterless constructor";
            DivideByZeroException = new DivideByZeroException("1/0", new AbandonedMutexException());
        }

        public string Info { get; set; }

        public DivideByZeroException DivideByZeroException { get; set; }
    }

    public class ExceptionTestSample
    {
        public void CreateInnerSystemExceptions()
        {
            try
            {
                var c = "dummy".ToCharArray()[int.MaxValue];
            }
            catch (IndexOutOfRangeException e1)
            {
                throw new ArgumentOutOfRangeException("x-axis", e1);
            }
        }

        public void CreateNotSerializableException()
        {
            var ex = new CustomException("This is a custom exception");
            ex.Data.Add("TheKey", "TheValue");
            throw ex;
        }
    }
}
