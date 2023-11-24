using Microsoft.VisualStudio.TestTools.UnitTesting;
using SantasToolbox;

namespace SantasToolBoxTests;

[TestClass]
public class UniqueFactoryTests
{
    [TestMethod]
    public void CallsConstructingFuncToGetOrCreateInstance()
    {
        // Arrange
        bool hasCalledConstructingFunc = false;
        int constructingFunc(string _) { hasCalledConstructingFunc = true; return 1; }
        var factory = new UniqueFactory<string, int>(constructingFunc);

        // Act
        var instance = factory.GetOrCreateInstance("1");

        // Assert
        Assert.IsTrue(hasCalledConstructingFunc);
    }

    [TestMethod]
    public void ConstructingFuncIsCalledOnceForSameId()
    {
        // Arrange
        int constructingFuncCount = 0;
        object constructingFunc(string _) { constructingFuncCount++; return new object(); }
        var factory = new UniqueFactory<string, object>(constructingFunc);

        // Act
        var instance1 = factory.GetOrCreateInstance("1");
        var instance2 = factory.GetOrCreateInstance("1");

        // Assert
        Assert.IsNotNull(instance1);
        Assert.IsNotNull(instance2);
        Assert.AreEqual(constructingFuncCount, 1);
    }

    [TestMethod]
    public void SameObjectIsReturnedForSameId()
    {
        // Arrange
        static object constructingFunc(string _) => new();
        var factory = new UniqueFactory<string, object>(constructingFunc);

        // Act
        var instance1 = factory.GetOrCreateInstance("1");
        var instance2 = factory.GetOrCreateInstance("1");

        // Assert
        Assert.AreEqual(instance1, instance2);
    }

    [TestMethod]
    public void DifferetObjectIsReturnedForDifferentId()
    {
        // Arrange
        static object constructingFunc(string _) => new();
        var factory = new UniqueFactory<string, object>(constructingFunc);

        // Act
        var instance1 = factory.GetOrCreateInstance("1");
        var instance2 = factory.GetOrCreateInstance("2");

        // Assert
        Assert.AreNotEqual(instance1, instance2);
    }

    [TestMethod]
    public void CreatedInstanceExistsInAllCreatedInstances()
    {
        // Arrange
        static object constructingFunc(string _) => new();
        var factory = new UniqueFactory<string, object>(constructingFunc);

        // Act
        var instance1 = factory.GetOrCreateInstance("1");
        var instance2 = factory.GetOrCreateInstance("3");

        // Assert
        Assert.IsTrue(factory.AllCreatedInstances.Contains(instance1));
        Assert.IsTrue(factory.AllCreatedInstances.Contains(instance2));
    }

    [TestMethod]
    public void AllCreatedInstancesDoesNotStoreDuplicateObjects()
    {
        // Arrange
        static object constructingFunc(string _) => new();
        var factory = new UniqueFactory<string, object>(constructingFunc);

        // Act
        var instance1 = factory.GetOrCreateInstance("1");
        var instance2 = factory.GetOrCreateInstance("1");
        var instance3 = factory.GetOrCreateInstance("3");

        // Assert
        Assert.AreEqual(factory.AllCreatedInstances.Count, 2);
    }
}
