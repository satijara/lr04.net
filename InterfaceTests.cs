using System.Collections.Generic;
using ArtCollection.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArtCollection.Tests;

[TestClass]
public class InterfaceTests
{
    private static List<Artwork> CreateCollection() => new List<Artwork>
    {
        new Painting("Картина", 1889, "Автор 1", 1000m, "олія", 50, 70),
        new Sculpture("Скульптура", 1890, "Автор 2", 1000m, "бронза"),
        new Photography("Фотографія", 2000, "Автор 3", 1000m, "Чорно-біла", 5)
    };

    [TestMethod]
    public void TotalValue_UsesInterfacePolymorphism()
    {
        IEnumerable<IEvaluable> items = CreateCollection();
        Assert.AreEqual(3750m, InterfaceAnalysis.TotalValue(items));
    }

    [TestMethod]
    public void PrepareExhibition_UsesDifferentImplementations()
    {
        var artworks = CreateCollection();
        var items = new List<IExhibitable>();
        foreach (Artwork artwork in artworks)
            if (artwork is IExhibitable exhibit) items.Add(exhibit);
        var result = InterfaceAnalysis.PrepareExhibition(items);
        Assert.AreEqual(3, result.Count);
        StringAssert.Contains(result[0], "раму");
        StringAssert.Contains(result[1], "постамент");
        StringAssert.Contains(result[2], "захисне скло");
    }

    [TestMethod]
    public void RestoreAll_UsesInterfaceAndSkipsRestored()
    {
        var artworks = CreateCollection();
        IEnumerable<IRestorable> items = artworks;
        Assert.AreEqual(3, InterfaceAnalysis.RestoreAll(items).Count);
        foreach (IRestorable item in items) Assert.IsTrue(item.IsRestored);
        Assert.AreEqual(3937.50m, InterfaceAnalysis.TotalValue(artworks));
        Assert.AreEqual(0, InterfaceAnalysis.RestoreAll(items).Count);
    }

    [TestMethod]
    public void TotalValue_EmptyCollection_ReturnsZero()
    {
        Assert.AreEqual(0m, InterfaceAnalysis.TotalValue(new List<IEvaluable>()));
    }
}
