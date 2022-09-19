// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

namespace YAXLibTests.SampleClasses
{
    using System;
    using System.Threading;

    namespace YAXLibTests.SampleClasses
    {
        public class CustomException : Exception
        {
            public CustomException(string message, Exception innerException) : base(message, innerException)
            {
            }

            public CustomException()
            {
                // required for deserialization
            }
        }

        public class ExceptionTestSamples
        {
            public static void ThrowCustomException()
            {
                var innerException = new DivideByZeroException("InnerException of CustomException",
                    new AbandonedMutexException("CustomException / DivideByZero / InnerException"));
                var ex = new CustomException("This is a custom exception", innerException);
                throw ex;
            }

            public static void ThrowInvalidOperationException(string arg = "arg")
            {
                _ = arg; // Make diagnostics happy
                throw new InvalidOperationException("System exception unit test",
                    new ArgumentException("Inner exception unit test", nameof(arg)));
            }
        }
    }
}
