using System.Collections.Generic;

namespace ArtCollection.Models;

public static class InterfaceAnalysis
{
    public static decimal TotalValue(IEnumerable<IEvaluable> items)
    {
        decimal total = 0;
        foreach (IEvaluable item in items)
            total += item.Evaluate();
        return total;
    }

    public static List<string> PrepareExhibition(IEnumerable<IExhibitable> items)
    {
        var result = new List<string>();
        foreach (IExhibitable item in items)
            result.Add(item.PrepareForExhibition());
        return result;
    }

    public static List<string> RestoreAll(IEnumerable<IRestorable> items)
    {
        var result = new List<string>();
        foreach (IRestorable item in items)
            if (!item.IsRestored)
                result.Add(item.Restore());
        return result;
    }
}