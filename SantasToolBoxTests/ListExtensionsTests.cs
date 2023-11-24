using Microsoft.VisualStudio.TestTools.UnitTesting;
using SantasToolbox;

namespace SantasToolBoxTests;

[TestClass]
public class ListExtensionsTests
{
    [TestMethod]
    public void SumsWindowLengthWhenWindowEqualThanList()
    {
        // Arrange
        var list = new List<int>() { 1, 2, 3 };

        // Act, Assert
        Assert.AreEqual(list.Sum(), list.GetSlidingWindowSum(2, 3));
    }

    [TestMethod]
    public void SumsWindowLengthWhenWindowLongerThanList()
    {
        // Arrange
        var list = new List<int>() { 1, 2, 3 };

        // Act, Assert
        Assert.AreEqual(list.Sum(), list.GetSlidingWindowSum(2, 5));
    }

    [TestMethod]
    public void SumsEqualFirstElementAtStartOfList()
    {
        // Arrange
        var list = new List<int>() { 1, 2, 3 };

        // Act, Assert
        Assert.AreEqual(1, list.GetSlidingWindowSum(0, 2));
    }

    [TestMethod]
    public void IgnoresItemsOutsideOfWindow()
    {
        // Arrange
        var list = new List<int>() { 1, 2, 3, 4 };

        // Act, Assert
        Assert.AreEqual(2 + 3, list.GetSlidingWindowSum(2, 2));
    }
}