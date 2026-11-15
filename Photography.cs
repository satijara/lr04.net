using System;

namespace ArtCollection.Models;

public class Photography : Artwork, IExhibitable
{
    public string PhotoType { get; }
    public int EditionSize { get; }

    public Photography(string title, int year, string author, decimal baseValue,
                       string photoType, int editionSize)
        : base(title, year, author, baseValue)
    {
        if (string.IsNullOrWhiteSpace(photoType))
            throw new ArgumentException("Вкажіть тип фотографії.", nameof(photoType));
        if (editionSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(editionSize), "Тираж має бути додатним.");
        PhotoType = photoType.Trim();
        EditionSize = editionSize;
    }

    public override decimal Evaluate()
    {
        decimal factor = EditionSize <= 10 ? 1.20m : 1m;
        return decimal.Round(BaseValue * factor * RestorationMultiplier, 2);
    }

    public override string Restore()
    {
        if (IsRestored) return "Фотографію вже реставровано.";
        IsRestored = true;
        return $"Фотографію «{Title}» реставровано.";
    }

    public override string Details() =>
        $"Фотографія | {PhotoType} | Тираж: {EditionSize}";

    public string PrepareForExhibition() =>
        $"Фотографія «{Title}»: підготувати захисне скло та розсіяне освітлення.";
}