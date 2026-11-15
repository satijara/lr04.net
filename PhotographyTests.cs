using System;
using ArtCollection.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArtCollection.Tests;

[TestClass]
public class PhotographyTests
{
    [TestMethod]
    public void Constructor_SetsProperties()
    {
        var photo = new Photography("Туман", 2000, "Автор", 1000m, "Чорно-біла", 10);
        Assert.AreEqual("Туман", photo.Title);
        Assert.AreEqual("Чорно-біла", photo.PhotoType);
        Assert.AreEqual(10, photo.EditionSize);
        Assert.IsFalse(photo.IsRestored);
    }

    [DataTestMethod]
    [DataRow(1, 1200)]
    [DataRow(10, 1200)]
    [DataRow(11, 1000)]
    public void Evaluate_DependsOnEdition(int edition, int expected)
    {
        var photo = new Photography("Туман", 2000, "Автор", 1000m, "Чорно-біла", edition);
        Assert.AreEqual((decimal)expected, photo.Evaluate());
    }

    [TestMethod]
    public void Restore_IncreasesValueOnlyOnce()
    {
        var photo = new Photography("Туман", 2000, "Автор", 1000m, "Чорно-біла", 5);
        photo.Restore();
        Assert.IsTrue(photo.IsRestored);
        Assert.AreEqual(1260m, photo.Evaluate());
        Assert.AreEqual("Фотографію вже реставровано.", photo.Restore());
        Assert.AreEqual(1260m, photo.Evaluate());
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    public void Constructor_InvalidEdition_Throws(int edition)
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            new Photography("Туман", 2000, "Автор", 1000m, "Чорно-біла", edition));
    }

    [TestMethod]
    public void Constructor_EmptyType_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            new Photography("Туман", 2000, "Автор", 1000m, " ", 5));
    }

    [TestMethod]
    public void Details_ContainsTypeAndEdition()
    {
        var photo = new Photography("Туман", 2000, "Автор", 1000m, "Чорно-біла", 5);
        Assert.AreEqual("Фотографія | Чорно-біла | Тираж: 5", photo.Details());
    }
}
