using ArtCollection.Models;
using ArtCollection.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ArtCollection;

public partial class MainWindow : Window
{
    private readonly List<Artwork> _artworks = new();
    private readonly CultureInfo _ua = CultureInfo.GetCultureInfo("uk-UA");

    public MainWindow()
    {
        InitializeComponent();
        if (TypeComboBox.SelectedIndex < 0)
            TypeComboBox.SelectedIndex = 0;
        UpdateTypeFields();
        UpdateList();
    }

    private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateTypeFields();
    }

    private void UpdateTypeFields()
    {
        if (PaintingFields is null || SculptureFields is null || PhotographyFields is null ||
            TitleTextBox is null || YearTextBox is null || AuthorTextBox is null ||
            BaseValueTextBox is null || TechniqueTextBox is null || WidthTextBox is null ||
            HeightTextBox is null || MaterialTextBox is null || PhotoTypeTextBox is null ||
            EditionSizeTextBox is null)
            return;

        int type = TypeComboBox.SelectedIndex;
        PaintingFields.Visibility = type == 0 ? Visibility.Visible : Visibility.Collapsed;
        SculptureFields.Visibility = type == 1 ? Visibility.Visible : Visibility.Collapsed;
        PhotographyFields.Visibility = type == 2 ? Visibility.Visible : Visibility.Collapsed;

        switch (type)
        {
            case 0:
                TitleTextBox.Text = "Зоряна ніч";
                YearTextBox.Text = "1889";
                AuthorTextBox.Text = "Вінсент ван Гог";
                BaseValueTextBox.Text = "10000";
                TechniqueTextBox.Text = "Олія";
                WidthTextBox.Text = "73.7";
                HeightTextBox.Text = "92.1";
                break;
            case 1:
                TitleTextBox.Text = "Мислитель";
                YearTextBox.Text = "1904";
                AuthorTextBox.Text = "Огюст Роден";
                BaseValueTextBox.Text = "15000";
                MaterialTextBox.Text = "Бронза";
                break;
            case 2:
                TitleTextBox.Text = "Туман";
                YearTextBox.Text = "2020";
                AuthorTextBox.Text = "Невідомий автор";
                BaseValueTextBox.Text = "5000";
                PhotoTypeTextBox.Text = "Чорно-біла";
                EditionSizeTextBox.Text = "5";
                break;
        }
    }

    private static decimal ParseMoney(string input)
    {
        string normalized = input.Trim().Replace(',', '.');
        if (!decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result))
            throw new FormatException("Введіть коректну базову вартість.");
        return result;
    }

    private static double ParseSize(string input)
    {
        string normalized = input.Trim().Replace(',', '.');
        if (!double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
            throw new FormatException("Введіть коректний розмір полотна.");
        return result;
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!int.TryParse(YearTextBox.Text, out int year))
                throw new FormatException("Рік створення повинен бути цілим числом.");

            decimal value = ParseMoney(BaseValueTextBox.Text);
            Artwork artwork = TypeComboBox.SelectedIndex switch
            {
                0 => new Painting(TitleTextBox.Text, year, AuthorTextBox.Text, value,
                    TechniqueTextBox.Text, ParseSize(WidthTextBox.Text), ParseSize(HeightTextBox.Text)),
                1 => new Sculpture(TitleTextBox.Text, year, AuthorTextBox.Text, value, MaterialTextBox.Text),
                2 => new Photography(TitleTextBox.Text, year, AuthorTextBox.Text, value,
                    PhotoTypeTextBox.Text, int.Parse(EditionSizeTextBox.Text)),
                _ => throw new ArgumentException("Оберіть тип твору.")
            };

            _artworks.Add(artwork);
            UpdateList();
            ArtworkListBox.SelectedIndex = _artworks.Count - 1;
            StatusTextBlock.Text = $"Додано: «{artwork.Title}».";
        }
        catch (Exception ex) when (ex is ArgumentException or FormatException or OverflowException)
        {
            MessageBox.Show(ex.Message, "Помилка введення", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private Artwork? SelectedArtwork => ArtworkListBox.SelectedIndex is int i && i >= 0 && i < _artworks.Count
        ? _artworks[i] : null;

    private void ArtworkListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SelectedInfoTextBlock is null) return;
        Artwork? artwork = SelectedArtwork;
        SelectedInfoTextBlock.Text = artwork is null
            ? "Оберіть твір, щоб переглянути його характеристики."
            : $"{artwork.Title} ({artwork.CreationYear}), {artwork.Author}\n{artwork.Details()}\nРеставрація: {(artwork.IsRestored ? "так" : "ні")}";
    }

    private void EvaluateButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedArtwork is not Artwork artwork) { ShowSelectionMessage(); return; }
        IEvaluable evaluable = artwork;
        StatusTextBlock.Text = $"Оцінка «{artwork.Title}»: {evaluable.Evaluate().ToString("N2", _ua)} грн.";
    }

    private void RestoreButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedArtwork is not Artwork artwork) { ShowSelectionMessage(); return; }
        IRestorable restorable = artwork;
        string message = restorable.Restore();
        UpdateList();
        ArtworkListBox.SelectedIndex = _artworks.IndexOf(artwork);
        StatusTextBlock.Text = message;
    }

    private void ExhibitionButton_Click(object sender, RoutedEventArgs e)
    {
        var result = InterfaceAnalysis.PrepareExhibition(_artworks.OfType<IExhibitable>());
        MessageBox.Show(result.Count == 0 ? "Колекція порожня." : string.Join(Environment.NewLine, result),
            "Підготовка до виставки", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void RestoreAllButton_Click(object sender, RoutedEventArgs e)
    {
        int selectedIndex = ArtworkListBox.SelectedIndex;
        var result = InterfaceAnalysis.RestoreAll(_artworks.OfType<IRestorable>());
        UpdateList();
        ArtworkListBox.SelectedIndex = selectedIndex;
        string message = _artworks.Count == 0 ? "Колекція порожня." :
            result.Count == 0 ? "Усі твори вже реставровано." : string.Join(Environment.NewLine, result);
        StatusTextBlock.Text = $"Реставровано творів: {result.Count}.";
        MessageBox.Show(message, "Реставрація колекції", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedArtwork is not Artwork artwork) { ShowSelectionMessage(); return; }
        _artworks.Remove(artwork);
        UpdateList();
        StatusTextBlock.Text = $"Видалено: «{artwork.Title}».";
    }

    private void ShowSelectionMessage() => MessageBox.Show("Спочатку оберіть твір у списку.",
        "Не вибрано твір", MessageBoxButton.OK, MessageBoxImage.Information);

    private void UpdateList()
    {
        ArtworkListBox.ItemsSource = null;
        ArtworkListBox.ItemsSource = _artworks.Select(a => new ArtworkItem(a)).ToList();
        TotalValueTextBlock.Text = CollectionAnalysis.TotalValueBefore1900(_artworks).ToString("N2", _ua) + " грн";
        AllValueTextBlock.Text = InterfaceAnalysis.TotalValue(_artworks).ToString("N2", _ua) + " грн";
        SelectedInfoTextBlock.Text = "Оберіть твір, щоб переглянути його характеристики.";
    }

    private sealed class ArtworkItem
    {
        public Artwork Artwork { get; }
        public ArtworkItem(Artwork artwork) => Artwork = artwork;
        public override string ToString()
        {
            string type = Artwork switch
            {
                Painting => "Картина",
                Sculpture => "Скульптура",
                Photography => "Фотографія",
                _ => "Твір"
            };
            return $"{type}: {Artwork.Title} ({Artwork.CreationYear}) — {Artwork.Evaluate():N2} грн";
        }
    }
}