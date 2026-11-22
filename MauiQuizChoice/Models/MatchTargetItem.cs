using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiQuizChoice.Models;

public partial class MatchTargetItem : ObservableObject
{
    public int PairId { get; set; }
    public string Text { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsMatched {get; set; }

    [ObservableProperty]
    public partial string MatchedBadgeText {get; set; } = string.Empty;
}
