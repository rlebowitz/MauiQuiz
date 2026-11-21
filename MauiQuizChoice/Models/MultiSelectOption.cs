using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiQuizChoice.Models;

public partial class MultiSelectOption : ObservableObject
{
    public string Text { get; set; } = string.Empty;
    public bool IsCorrectOption { get; set; }

    [ObservableProperty]
    private bool _isSelected;
}

public class MultiSelectQuestion
{
    public string Text { get; set; } = string.Empty;
    public ObservableCollection<MultiSelectOption> Options { get; set; } = new();
}