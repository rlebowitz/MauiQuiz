using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiQuizChoice.Models;

public partial class AnswerOption : ObservableObject
{
    public string Text { get; set; } = string.Empty;

    [ObservableProperty]
    private bool _isSelected;
}
