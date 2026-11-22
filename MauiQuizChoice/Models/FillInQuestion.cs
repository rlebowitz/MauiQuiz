using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiQuizChoice.Models;

public partial class FillInQuestion : ObservableObject
{
    public string PromptBeforeBlank { get; set; } = string.Empty;
    public string PromptAfterBlank { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public string Hint { get; set; } = string.Empty;

    [ObservableProperty]
    private string _userInput = string.Empty;
}