using System;
using System.Reflection;
using MFUnitTest;

namespace Tests.Common.Assertions
{
    class Program
    {
        public static void Main()
        {
            // Run all tests in current assembly
            TestManager.RunTests(Assembly.GetExecutingAssembly());

            // Run all tests for specified Test Class
            //TestManager.RunTest(typeof(BooleanAssertionTests));

            // Run specified test for specified Test Class
            //TestManager.RunTest(typeof(BooleanAssertionTests), "TestMethod1");
        }
    }
}
