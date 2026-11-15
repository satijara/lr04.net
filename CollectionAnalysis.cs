using System.Collections.Generic;
using System.Linq;
using ArtCollection.Models;

namespace ArtCollection.Services;

public static class CollectionAnalysis
{
    // Загальна оціночна вартість творів, створених до 1900 року.
    public static decimal TotalValueBefore1900(IEnumerable<Artwork> artworks)
    {
        return artworks.Where(a => a.CreationYear < 1900).Sum(a => a.Evaluate());
    }
}
