using CoordinateSharp;
using NUnit.Framework;

namespace App.Tests;

public class CoordinateTests
{
    [SetUp]
    public void Setup() { }

    [Test]
    public void CoordinateParsingTest()
    {
        var success = Coordinate.TryParse("50°58'07.9\"N 0°29'55.9\"W", out var c);
            
        Assert.That(success, Is.True);
        Assert.That(c, Is.Not.Null);
        Assert.That(c.Latitude.DecimalDegree, Is.EqualTo(50.96886111111111d));
        Assert.That(c.Longitude.DecimalDegree, Is.EqualTo(-0.49886111111111109d));
    }
}