using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiQuizChoice.Models;

public partial class TrueFalseQuestion : ObservableObject
{
    public string Statement { get; set; } = string.Empty;
    public bool CorrectAnswer { get; set; }

    [ObservableProperty]
    public partial bool IsTrueSelected { get; set; }

    [ObservableProperty]
    public partial bool IsFalseSelected { get; set; }
}