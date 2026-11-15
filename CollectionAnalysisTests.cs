using ArtCollection.Models;
using ArtCollection.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ArtCollection.Tests;

[TestClass]
public class CollectionAnalysisTests
{
    [TestMethod]
    public void Before1900_ExcludesBoundaryAndLaterWorks()
    {
        var items = new List<Artwork>
        {
            new Painting("Картина", 1889, "Автор", 1000m, "олія", 50, 70),
            new Sculpture("Скульптура", 1899, "Автор", 1000m, "бронза"),
            new Sculpture("Межа", 1900, "Автор", 1000m, "мармур"),
            new Photography("Фото", 2000, "Автор", 1000m, "Чорно-біла", 5)
        };
        Assert.AreEqual(2550m, CollectionAnalysis.TotalValueBefore1900(items));
    }

    [TestMethod]
    public void Painting_Restoration_UpdatesValue()
    {
        var work = new Painting("Картина", 1889, "Автор", 1000m, "олія", 50, 70);
        Assert.AreEqual(1250m, work.Evaluate());
        work.Restore();
        Assert.AreEqual(1312.50m, work.Evaluate());
    }

    [TestMethod]
    public void Sculpture_UsesMaterialAndAge()
    {
        var work = new Sculpture("Скульптура", 1904, "Автор", 1000m, "бронза");
        Assert.AreEqual(1300m, work.Evaluate());
        Assert.AreEqual(100, work.EstimateAge(2004));
    }
}
