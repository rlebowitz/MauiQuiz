using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiQuizChoice.Models;
using System.Collections.ObjectModel;

namespace MauiQuizChoice.ViewModels;

public record MultiSelectReviewItem(
    string QuestionText,
    string UserSelectionsText,
    string CorrectSelectionsText,
    bool IsExactMatch);

public partial class MultiSelectQuizViewModel : ObservableObject
{
    private readonly List<MultiSelectQuestion> _questions;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressValue))]
    [NotifyPropertyChangedFor(nameof(ProgressText))]
    [NotifyPropertyChangedFor(nameof(CanGoBack))]
    [NotifyCanExecuteChangedFor(nameof(PreviousCommand))]
    private int _currentIndex;

    [ObservableProperty]
    private MultiSelectQuestion _currentQuestion;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextCommand))]
    private bool _isNextEnabled;

    [ObservableProperty]
    private bool _isAnalyzing;

    [ObservableProperty]
    private bool _isSummaryVisible;

    [ObservableProperty]
    private string _scoreSummaryText = string.Empty;

    public ObservableCollection<MultiSelectReviewItem> ReviewResults { get; } = new();

    public MultiSelectQuizViewModel()
    {
        _questions = new List<MultiSelectQuestion>
        {
            new MultiSelectQuestion
            {
                Text = "Which of the following are valid layout panels in .NET MAUI? (Select all that apply)",
                Options = new()
                {
                    new MultiSelectOption { Text = "Grid", IsCorrectOption = true },
                    new MultiSelectOption { Text = "FlexLayout", IsCorrectOption = true },
                    new MultiSelectOption { Text = "LinearLayout", IsCorrectOption = false },
                    new MultiSelectOption { Text = "VerticalStackLayout", IsCorrectOption = true }
                }
            },
            new MultiSelectQuestion
            {
                Text = "Which binding modes exist in .NET MAUI XAML?",
                Options = new()
                {
                    new MultiSelectOption { Text = "OneWay", IsCorrectOption = true },
                    new MultiSelectOption { Text = "TwoWay", IsCorrectOption = true },
                    new MultiSelectOption { Text = "OneTime", IsCorrectOption = true },
                    new MultiSelectOption { Text = "ReverseOnly", IsCorrectOption = false }
                }
            },
            new MultiSelectQuestion
            {
                Text = "Which attributes come from CommunityToolkit.Mvvm?",
                Options = new()
                {
                    new MultiSelectOption { Text = "[ObservableProperty]", IsCorrectOption = true },
                    new MultiSelectOption { Text = "[RelayCommand]", IsCorrectOption = true },
                    new MultiSelectOption { Text = "[InjectService]", IsCorrectOption = false },
                    new MultiSelectOption { Text = "[NotifyPropertyChangedFor]", IsCorrectOption = true }
                }
            }
        };

        _currentQuestion = _questions[0];
    }

    public double ProgressValue => (double)(CurrentIndex + 1) / _questions.Count;
    public string ProgressText => $"{CurrentIndex + 1}/{_questions.Count}";
    public bool CanGoBack => CurrentIndex > 0;

    [RelayCommand]
    private void ToggleOption(MultiSelectOption? option)
    {
        if (option is null) return;

        option.IsSelected = !option.IsSelected;
        IsNextEnabled = CurrentQuestion.Options.Any(o => o.IsSelected);
    }

    [RelayCommand(CanExecute = nameof(IsNextEnabled))]
    private async Task Next()
    {
        if (CurrentIndex < _questions.Count - 1)
        {
            CurrentIndex++;
            CurrentQuestion = _questions[CurrentIndex];
            IsNextEnabled = CurrentQuestion.Options.Any(o => o.IsSelected);
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
        CurrentQuestion = _questions[CurrentIndex];
        IsNextEnabled = CurrentQuestion.Options.Any(o => o.IsSelected);
    }

    [RelayCommand]
    private void Restart()
    {
        foreach (var q in _questions)
        {
            foreach (var opt in q.Options)
            {
                opt.IsSelected = false;
            }
        }

        ReviewResults.Clear();
        IsSummaryVisible = false;
        IsAnalyzing = false;
        CurrentIndex = 0;
        CurrentQuestion = _questions[0];
        IsNextEnabled = false;
    }

    private void BuildReview()
    {
        ReviewResults.Clear();
        int exactMatches = 0;

        foreach (var q in _questions)
        {
            var selected = q.Options.Where(o => o.IsSelected).Select(o => o.Text).ToList();
            var correct = q.Options.Where(o => o.IsCorrectOption).Select(o => o.Text).ToList();

            bool isExact = q.Options.All(o => o.IsSelected == o.IsCorrectOption);
            if (isExact) exactMatches++;

            string userText = selected.Count > 0 ? string.Join(", ", selected) : "None selected";
            string correctText = string.Join(", ", correct);

            ReviewResults.Add(new MultiSelectReviewItem(q.Text, userText, correctText, isExact));
        }

        ScoreSummaryText = $"You answered {exactMatches} of {_questions.Count} questions completely right";
    }
}