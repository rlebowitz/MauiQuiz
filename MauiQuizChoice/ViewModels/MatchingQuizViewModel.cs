using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiQuizChoice.Models;
using System.Collections.ObjectModel;

namespace MauiQuizChoice.ViewModels;

public record MatchReviewItem(string Prompt, string UserMatch, string CorrectMatch, bool IsCorrect);

public partial class MatchingQuizViewModel : ObservableObject
{
    private readonly List<MatchingRound> _rounds;
    private readonly Dictionary<int, int> _userSelections = new(); // PromptPairId -> TargetPairId

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressValue))]
    [NotifyPropertyChangedFor(nameof(ProgressText))]
    [NotifyPropertyChangedFor(nameof(CanGoBack))]
    [NotifyCanExecuteChangedFor(nameof(PreviousCommand))]
    private int _currentIndex;

    [ObservableProperty]
    private MatchingRound _currentRound;

    [ObservableProperty]
    private MatchPromptItem? _activePrompt;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextCommand))]
    private bool _isNextEnabled;

    [ObservableProperty]
    private bool _isAnalyzing;

    [ObservableProperty]
    private bool _isSummaryVisible;

    [ObservableProperty]
    private string _scoreSummaryText = string.Empty;

    public ObservableCollection<MatchReviewItem> ReviewResults { get; } = new();

    public MatchingQuizViewModel()
    {
        _rounds = new List<MatchingRound>
        {
            new MatchingRound
            {
                Instruction = "Match each .NET MAUI layout with its primary behavior.",
                Prompts = new()
                {
                    new MatchPromptItem { PairId = 1, Text = "Grid" },
                    new MatchPromptItem { PairId = 2, Text = "VerticalStackLayout" },
                    new MatchPromptItem { PairId = 3, Text = "CollectionView" }
                },
                Targets = new()
                {
                    new MatchTargetItem { PairId = 2, Text = "Stacks child views in a single column" },
                    new MatchTargetItem { PairId = 3, Text = "Renders virtualized scrollable lists" },
                    new MatchTargetItem { PairId = 1, Text = "Arranges views into rows and columns" }
                }
            },
            new MatchingRound
            {
                Instruction = "Match each MVVM attribute with its generated output.",
                Prompts = new()
                {
                    new MatchPromptItem { PairId = 4, Text = "[ObservableProperty]" },
                    new MatchPromptItem { PairId = 5, Text = "[RelayCommand]" },
                    new MatchPromptItem { PairId = 6, Text = "[BindableProperty]" }
                },
                Targets = new()
                {
                    new MatchTargetItem { PairId = 6, Text = "Creates XAML-bindable control properties" },
                    new MatchTargetItem { PairId = 4, Text = "Generates INotifyPropertyChanged boilerplate" },
                    new MatchTargetItem { PairId = 5, Text = "Wraps a method into an ICommand" }
                }
            }
        };

        _currentRound = _rounds[0];
    }

    public double ProgressValue => (double)(CurrentIndex + 1) / _rounds.Count;
    public string ProgressText => $"{CurrentIndex + 1}/{_rounds.Count}";
    public bool CanGoBack => CurrentIndex > 0;

    [RelayCommand]
    private void SelectPrompt(MatchPromptItem prompt)
    {
        if (prompt == null) return;

        // If already matched, unpair it first so the user can re-match
        if (prompt.IsMatched)
        {
            UnpairPrompt(prompt);
        }

        foreach (var p in CurrentRound.Prompts)
            p.IsActive = (p == prompt);

        ActivePrompt = prompt;
    }

    [RelayCommand]
    private void SelectTarget(MatchTargetItem target)
    {
        if (target == null || ActivePrompt == null) return;

        // If target was already claimed by another prompt, release that previous pairing
        var existingPrompt = CurrentRound.Prompts
            .FirstOrDefault(p => p.IsMatched && p.MatchedTargetText == target.Text);
        if (existingPrompt != null)
        {
            UnpairPrompt(existingPrompt);
        }

        int badgeNumber = CurrentRound.Prompts.IndexOf(ActivePrompt) + 1;
        string badge = badgeNumber.ToString();

        ActivePrompt.IsMatched = true;
        ActivePrompt.IsActive = false;
        ActivePrompt.MatchedBadgeText = badge;
        ActivePrompt.MatchedTargetText = target.Text;

        target.IsMatched = true;
        target.MatchedBadgeText = badge;

        _userSelections[ActivePrompt.PairId] = target.PairId;
        ActivePrompt = null;

        // Enable Next once all prompts in this round are paired
        IsNextEnabled = CurrentRound.Prompts.All(p => p.IsMatched);

        // Auto-select the next unmatched prompt for fast flow
        var nextUnmatched = CurrentRound.Prompts.FirstOrDefault(p => !p.IsMatched);
        if (nextUnmatched != null)
        {
            SelectPrompt(nextUnmatched);
        }
    }

    private void UnpairPrompt(MatchPromptItem prompt)
    {
        var pairedTarget = CurrentRound.Targets
            .FirstOrDefault(t => t.IsMatched && t.Text == prompt.MatchedTargetText);

        if (pairedTarget != null)
        {
            pairedTarget.IsMatched = false;
            pairedTarget.MatchedBadgeText = string.Empty;
        }

        prompt.IsMatched = false;
        prompt.MatchedBadgeText = string.Empty;
        prompt.MatchedTargetText = string.Empty;
        _userSelections.Remove(prompt.PairId);
        IsNextEnabled = false;
    }

    [RelayCommand(CanExecute = nameof(IsNextEnabled))]
    private async Task Next()
    {
        if (CurrentIndex < _rounds.Count - 1)
        {
            CurrentIndex++;
            CurrentRound = _rounds[CurrentIndex];
            ActivePrompt = null;
            IsNextEnabled = CurrentRound.Prompts.All(p => p.IsMatched);
        }
        else
        {
            IsAnalyzing = true;
            await Task.Delay(2000);

            BuildReview();
            IsAnalyzing = false;
            IsSummaryVisible = true;
        }
    }

    [RelayCommand(CanExecute = nameof(CanGoBack))]
    private void Previous()
    {
        if (CurrentIndex <= 0) return;

        CurrentIndex--;
        CurrentRound = _rounds[CurrentIndex];
        ActivePrompt = null;
        IsNextEnabled = CurrentRound.Prompts.All(p => p.IsMatched);
    }

    [RelayCommand]
    private void Restart()
    {
        _userSelections.Clear();
        foreach (var round in _rounds)
        {
            foreach (var p in round.Prompts)
            {
                p.IsActive = false;
                p.IsMatched = false;
                p.MatchedBadgeText = string.Empty;
                p.MatchedTargetText = string.Empty;
            }
            foreach (var t in round.Targets)
            {
                t.IsMatched = false;
                t.MatchedBadgeText = string.Empty;
            }
        }

        ReviewResults.Clear();
        ActivePrompt = null;
        IsSummaryVisible = false;
        IsAnalyzing = false;
        CurrentIndex = 0;
        CurrentRound = _rounds[0];
        IsNextEnabled = false;
    }

    [RelayCommand]
    private static async Task CloseQuiz()
    {
        await Shell.Current.GoToAsync("..");
    }

    private void BuildReview()
    {
        ReviewResults.Clear();
        int correctCount = 0;
        int totalCount = 0;

        foreach (var round in _rounds)
        {
            foreach (var prompt in round.Prompts)
            {
                totalCount++;
                _userSelections.TryGetValue(prompt.PairId, out int chosenTargetId);

                string userAnswer = round.Targets.FirstOrDefault(t => t.PairId == chosenTargetId)?.Text ?? "Unmatched";
                string correctAnswer = round.Targets.First(t => t.PairId == prompt.PairId).Text;
                bool isCorrect = chosenTargetId == prompt.PairId;

                if (isCorrect) correctCount++;
                ReviewResults.Add(new MatchReviewItem(prompt.Text, userAnswer, correctAnswer, isCorrect));
            }
        }

        ScoreSummaryText = $"You matched {correctCount} of {totalCount} items correctly";
    }
}