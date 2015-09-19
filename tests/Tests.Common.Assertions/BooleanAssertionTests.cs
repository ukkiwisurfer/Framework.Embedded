using System;

namespace Tests.Common.Assertions
{
    using Ignite.Framework.Micro.Common.Assertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BooleanAssertionTests
    {
        [TestMethod]
        public void ShouldBeTrue_ReturnsTrue_When_True()
        {
            BooleanAssertions.ShouldBeTrue(true);
        }

        [TestMethod]
        public void ShouldBeTrue_Throws_When_False()
        {
            try
            {
                BooleanAssertions.ShouldBeTrue(false);
                Assert.Fail();
            }
            catch (Exception e)
            {
                // Assertion exception expected
                e.OfType(typeof (ArgumentException));
            }
        }
    }
}
