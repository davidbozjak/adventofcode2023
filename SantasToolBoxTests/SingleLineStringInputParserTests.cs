using Microsoft.VisualStudio.TestTools.UnitTesting;
using SantasToolbox;

namespace SantasToolBoxTests;

[TestClass]
public class SingleLineStringInputParserTests
{
    [TestMethod]
    public void ReturnsFalseOnSingleNullInput()
    {
        // Arrange
        var parser = new SingleLineStringInputParser<int>(int.TryParse);

        // Act, Assert
        Assert.IsFalse(parser.GetValue(null, out int value));
    }

    [TestMethod]
    public void ReturnsFalseOnSingleEmptyStringInput()
    {
        // Arrange
        var parser = new SingleLineStringInputParser<int>(int.TryParse);

        // Act, Assert
        Assert.IsFalse(parser.GetValue(string.Empty, out int value));
    }

    [TestMethod]
    public void ReturnsFalseOnSingleNonIntInput()
    {
        // Arrange
        var parser = new SingleLineStringInputParser<int>(int.TryParse);

        // Act, Assert
        Assert.IsFalse(parser.GetValue("abc", out int value));
    }

    [TestMethod]
    public void ParsesSingleInput()
    {
        // Arrange
        var parser = new SingleLineStringInputParser<int>(int.TryParse);

        // Act, Assert
        Assert.IsTrue(parser.GetValue("1", out int value));
        Assert.AreEqual(value, 1);
    }

    [TestMethod]
    public void ParsesSingleInputWithWhitespace()
    {
        // Arrange
        var parser = new SingleLineStringInputParser<int>(int.TryParse);

        // Act, Assert
        Assert.IsTrue(parser.GetValue(" 1 ", out int value));
        Assert.AreEqual(value, 1);
    }

    [TestMethod]
    public void ParsesSingleInputWithDelimiter()
    {
        // Arrange
        var parser = new SingleLineStringInputParser<int>(int.TryParse);

        // Act, Assert

        Assert.IsTrue(parser.GetValue("1,", out int value));
        Assert.AreEqual(value, 1);
    }

    [TestMethod]
    public void ParsesSingleInputWithDelimiterAndWhitespace()
    {
        // Arrange
        var parser = new SingleLineStringInputParser<int>(int.TryParse);

        // Act, Assert

        Assert.IsTrue(parser.GetValue(" 1, ", out int value));
        Assert.AreEqual(value, 1);
    }

    [TestMethod]
    public void ReturnsFalseAfterDepleted()
    {
        // Arrange
        var parser = new SingleLineStringInputParser<int>(int.TryParse);

        // Act, Assert
        Assert.IsTrue(parser.GetValue("1", out int value1));
        Assert.IsFalse(parser.GetValue(null, out int value2));
    }

    [TestMethod]
    public void InputLineWith3IntsReads3Ints()
    {
        // Arrange
        var parser = new SingleLineStringInputParser<int>(int.TryParse);

        // Act, Assert
        parser.GetValue("1, 2, 3", out int value1);
        Assert.AreEqual(value1, 1);

        parser.GetValue(null, out int value2);
        Assert.AreEqual(value2, 2);

        parser.GetValue(null, out int value3);
        Assert.AreEqual(value3, 3);

        Assert.IsFalse(parser.GetValue(null, out int value4));
    }

    [TestMethod]
    public void ReturnsSeveralConsecutiveSingleInputs()
    {
        // Arrange
        var parser = new SingleLineStringInputParser<int>(int.TryParse);

        // Act, Assert
        parser.GetValue("1", out int value1);
        Assert.AreEqual(value1, 1);

        parser.GetValue("2", out int value2);
        Assert.AreEqual(value2, 2);

        Assert.IsFalse(parser.GetValue(null, out int value3));
    }

    [TestMethod]
    public void ReturnsSeveralConsecutiveSingleInputsWithDelimiters()
    {
        // Arrange
        var parser = new SingleLineStringInputParser<int>(int.TryParse);

        // Act, Assert
        parser.GetValue("1,", out int value1);
        Assert.AreEqual(value1, 1);

        parser.GetValue("2, ", out int value2);
        Assert.AreEqual(value2, 2);

        Assert.IsFalse(parser.GetValue(null, out int value3));
    }

    [TestMethod]
    public void ReturnsSeveralConsecutiveSingleInputsWithWhitespace()
    {
        // Arrange
        var parser = new SingleLineStringInputParser<int>(int.TryParse);

        // Act, Assert
        parser.GetValue("1,2", out int value1);
        Assert.AreEqual(value1, 1);

        parser.GetValue("3, ", out int value2);
        Assert.AreEqual(value2, 2);

        Assert.IsTrue(parser.GetValue(null, out int value3));
        Assert.AreEqual(value3, 3);

        Assert.IsFalse(parser.GetValue(null, out int value4));
    }
}
