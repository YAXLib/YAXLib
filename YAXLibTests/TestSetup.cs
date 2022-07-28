using System;
using System.IO;
using NUnit.Framework;
using YAXLib.Pooling;

namespace YAXLibTests;

[SetUpFixture]
public class TestSetup
{
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        // Disable console output from test methods
        Console.SetOut(TextWriter.Null);

        // Should be enabled for debugging
        PoolSettings.CheckReturnedObjectsExistInPool = true;
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
        // Nothing defined here
    }
}
