namespace ArtCollection.Models;

public interface IRestorable
{
    bool IsRestored { get; }
    string Restore();
}