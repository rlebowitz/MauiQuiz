using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiQuizChoice.Models;

public partial class FlashcardItem : ObservableObject
{
    public string Category { get; set; } = string.Empty;
    public string FrontText { get; set; } = string.Empty;
    public string BackText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsFlipped { get; set; }

    [ObservableProperty]
    public partial bool? Known { get; set; }
}
