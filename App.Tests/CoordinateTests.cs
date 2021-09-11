using CoordinateSharp;
using NUnit.Framework;

namespace App.Tests
{
    public class CoordinateTests
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public void CoordinateParsingTest()
        {
            Coordinate.TryParse("50°58'07.9\"N 0°29'55.9\"W", out Coordinate c);
            
            Assert.IsNotNull(c);
            Assert.AreEqual(50.96886111111111d, c.Latitude.DecimalDegree);
            Assert.AreEqual(-0.49886111111111109d, c.Longitude.DecimalDegree);
        }
    }
}