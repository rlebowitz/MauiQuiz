using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiQuizChoice.Models;

public partial class TrueFalseQuestion : ObservableObject
{
    public string Statement { get; set; } = string.Empty;
    public bool CorrectAnswer { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTrueSelected))]
    [NotifyPropertyChangedFor(nameof(IsFalseSelected))]
    private bool? _userAnswer;

    public bool IsTrueSelected => UserAnswer == true;
    public bool IsFalseSelected => UserAnswer == false;
}