using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiQuizChoice.Models;

public partial class TrueFalseQuestion : ObservableObject
{
    public string Statement { get; set; } = string.Empty;
    public bool CorrectAnswer { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTrueSelected))]
    [NotifyPropertyChangedFor(nameof(IsFalseSelected))]
    public partial bool? SelectedAnswer { get; set; }

    public bool IsTrueSelected {get { return SelectedAnswer == true; } set { SelectedAnswer = value; } }
    public bool IsFalseSelected {get { return SelectedAnswer == false; } set { SelectedAnswer = value; } }
}