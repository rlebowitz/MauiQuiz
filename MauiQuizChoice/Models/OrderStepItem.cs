using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiQuizChoice.Models;

public partial class OrderStepItem : ObservableObject
{
    public string Text { get; set; } = string.Empty;
    public int CorrectPosition { get; set; }

    [ObservableProperty]
    public partial int DisplayNumber { get; set; }
}
