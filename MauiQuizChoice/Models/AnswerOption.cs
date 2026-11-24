using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiQuizChoice.Models;

public partial class MultipleChoiceAnswerOption : ObservableObject
{
    public string Text { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsSelected {get; set; }
}
