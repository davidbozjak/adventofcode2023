using Microsoft.VisualStudio.TestTools.UnitTesting;
using SantasToolbox;

namespace SantasToolBoxTests;

[TestClass]
public class ClosedIntervalTests
{
    [TestMethod]
    public void ConstructSucceedes()
    {
        // Arrange
        var interval = new ClosedInterval(1, 2);

        // Act, Assert
        Assert.AreEqual(1, interval.Start);
        Assert.AreEqual(2, interval.End);
    }

    [TestMethod]
    public void ZeroLengthConstructDoesntThrow()
    {
        // Arrange
        var interval = new ClosedInterval(1, 1);

        // Act, Assert
        Assert.AreEqual(0, interval.Length);
    }

    [TestMethod]
    public void EndLowerThanStartGetsSwapedAfterConstruct()
    {
        // Arrange
        var interval = new ClosedInterval(2, 1);

        // Act, Assert
        Assert.AreEqual(1, interval.Start);
        Assert.AreEqual(2, interval.End);
    }

    [TestMethod]
    public void LengthDoesntThrow()
    {
        // Arrange
        var interval = new ClosedInterval(2, 1);

        // Act, Assert
        Assert.AreEqual(1, interval.Length);
    }

    [TestMethod]
    public void LengthOnNegativeRangeDoesntThrow()
    {
        // Arrange
        var interval = new ClosedInterval(-5, -1);

        // Act, Assert
        Assert.AreEqual(4, interval.Length);
    }

    [TestMethod]
    public void CenterPointDoesntThrow()
    {
        // Arrange
        var interval = new ClosedInterval(9, 1);

        // Act, Assert
        Assert.AreEqual(5, interval.CenterPoint);
    }

    [TestMethod]
    public void CenterPointOnNegativeRangeDoesntThrow()
    {
        // Arrange
        var interval = new ClosedInterval(-5, -1);

        // Act, Assert
        Assert.AreEqual(-3, interval.CenterPoint);
    }

    [TestMethod]
    public void HasIntervalTrue()
    {
        // Arrange
        var interval1 = new ClosedInterval(1, 10);
        var interval2 = new ClosedInterval(2, 11);

        // Act, Assert
        Assert.IsTrue(interval1.HasIntersect(interval2));
    }

    [TestMethod]
    public void HasIntervalFalse()
    {
        // Arrange
        var interval1 = new ClosedInterval(1, 10);
        var interval2 = new ClosedInterval(11, 13);

        // Act, Assert
        Assert.IsFalse(interval1.HasIntersect(interval2));
    }

    [TestMethod]
    public void CoversWholeIntervalTrue()
    {
        // Arrange
        var interval1 = new ClosedInterval(1, 10);
        var interval2 = new ClosedInterval(2, 9);

        // Act, Assert
        Assert.IsTrue(interval1.CoversWholeInterval(interval2));
    }

    [TestMethod]
    public void CoversWholeIntervalFalse()
    {
        // Arrange
        var interval1 = new ClosedInterval(1, 10);
        var interval2 = new ClosedInterval(10, 11);

        // Act, Assert
        Assert.IsFalse(interval1.CoversWholeInterval(interval2));
    }

    [TestMethod]
    public void ContainsPointTrue()
    {
        // Arrange
        var interval = new ClosedInterval(1, 10);
        var point = 9;

        // Act, Assert
        Assert.IsTrue(interval.ContainsPoint(point));
    }

    [TestMethod]
    public void ContainsPointOnEdgeTrue()
    {
        // Arrange
        var start = 1;
        var end = 10;
        var interval = new ClosedInterval(start, end);

        // Act, Assert
        Assert.IsTrue(interval.ContainsPoint(start));
        Assert.IsTrue(interval.ContainsPoint(end));
    }

    [TestMethod]
    public void ContainsPointFalse()
    {
        // Arrange
        var interval = new ClosedInterval(1, 10);
        var pointLeft = interval.Start - 1;
        var pointRight = interval.End + 1;

        // Act, Assert
        Assert.IsFalse(interval.ContainsPoint(pointLeft));
        Assert.IsFalse(interval.ContainsPoint(pointRight));
    }

    [TestMethod]
    public void UnionThrowsWhenNoIntersectExist()
    {
        // Arrange
        var interval1 = new ClosedInterval(1, 10);
        var interval2 = new ClosedInterval(11, 12);

        // Act, Assert
        Assert.ThrowsException<Exception>(() => interval1.Union(interval2));
    }

    [TestMethod]
    public void UnionSuccedes()
    {
        // Arrange
        var interval1 = new ClosedInterval(1, 10);
        var interval2 = new ClosedInterval(9, 12);

        // Act
        var unionInterval = interval1.Union(interval2);

        // Assert
        Assert.AreEqual(1, unionInterval.Start);
        Assert.AreEqual(12, unionInterval.End);
        Assert.AreEqual(11, unionInterval.Length);
    }

    [TestMethod]
    public void IntersectThrowsWhenNoIntersectExist()
    {
        // Arrange
        var interval1 = new ClosedInterval(1, 10);
        var interval2 = new ClosedInterval(11, 12);

        // Act, Assert
        Assert.ThrowsException<Exception>(() => interval1.Intersect(interval2));
    }

    [TestMethod]
    public void IntersectSuccedes()
    {
        // Arrange
        var interval1 = new ClosedInterval(1, 10);
        var interval2 = new ClosedInterval(9, 12);

        // Act
        var intersectInterval = interval1.Intersect(interval2);

        // Assert
        Assert.AreEqual(9, intersectInterval.Start);
        Assert.AreEqual(10, intersectInterval.End);
        Assert.AreEqual(1, intersectInterval.Length);
    }
}