using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiQuizChoice.Models;

public partial class MatchPromptItem : ObservableObject
{
    public int PairId { get; set; }
    public string Text { get; set; } = string.Empty;

    [ObservableProperty]
    private bool _isActive;

    [ObservableProperty]
    private bool _isMatched;

    [ObservableProperty]
    private string _matchedBadgeText = string.Empty;

    [ObservableProperty]
    private string _matchedTargetText = string.Empty;
}

public partial class MatchTargetItem : ObservableObject
{
    public int PairId { get; set; }
    public string Text { get; set; } = string.Empty;

    [ObservableProperty]
    private bool _isMatched;

    [ObservableProperty]
    private string _matchedBadgeText = string.Empty;
}

public class MatchingRound
{
    public string Instruction { get; set; } = string.Empty;
    public List<MatchPromptItem> Prompts { get; set; } = new();
    public List<MatchTargetItem> Targets { get; set; } = new();
}