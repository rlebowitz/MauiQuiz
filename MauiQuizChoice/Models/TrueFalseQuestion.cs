using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiQuizChoice.Models;

public partial class TrueFalseQuestion : ObservableObject
{
    public string Statement { get; set; } = string.Empty;
    public bool CorrectAnswer { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTrueSelected))]
    [NotifyPropertyChangedFor(nameof(IsFalseSelected))]
    public partial bool? UserAnswer { get; set; }

    public bool IsTrueSelected {get { return UserAnswer == true; } set { UserAnswer = value; } }
    public bool IsFalseSelected {get { return UserAnswer == false; } set { UserAnswer = value; } }
}