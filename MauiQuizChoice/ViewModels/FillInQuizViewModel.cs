using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiQuizChoice.Models;
using System.Collections.ObjectModel;

namespace MauiQuizChoice.ViewModels;

public record FillInReviewItem(
    string FullSentence,
    string UserAnswer,
    string CorrectAnswer,
    bool IsCorrect);

public partial class FillInQuizViewModel : ObservableObject
{
    private readonly List<FillInQuestion> _questions;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressValue))]
    [NotifyPropertyChangedFor(nameof(ProgressText))]
    [NotifyPropertyChangedFor(nameof(CanGoBack))]
    [NotifyCanExecuteChangedFor(nameof(PreviousCommand))]
    private int _currentIndex;

    [ObservableProperty]
    private FillInQuestion _currentQuestion;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextCommand))]
    private bool _isNextEnabled;

    [ObservableProperty]
    private bool _isAnalyzing;

    [ObservableProperty]
    private bool _isSummaryVisible;

    [ObservableProperty]
    private string _scoreSummaryText = string.Empty;

    public ObservableCollection<FillInReviewItem> ReviewResults { get; } = [];

    public FillInQuizViewModel()
    {
        _questions =
        [
            new FillInQuestion
            {
                PromptBeforeBlank = "In .NET MAUI, a GraphicsView requires calling ",
                PromptAfterBlank = "() to trigger a canvas redraw.",
                CorrectAnswer = "Invalidate",
                Hint = "Method on IGraphicsView"
            },
            new FillInQuestion
            {
                PromptBeforeBlank = "The ",
                PromptAfterBlank = " attribute generates ICommand properties from methods.",
                CorrectAnswer = "RelayCommand",
                Hint = "CommunityToolkit.Mvvm attribute"
            },
            new FillInQuestion
            {
                PromptBeforeBlank = "Use ",
                PromptAfterBlank = " mode when UI edits must update the ViewModel property.",
                CorrectAnswer = "TwoWay",
                Hint = "Default for Entry.Text"
            }
        ];

        _currentQuestion = _questions[0];
        HookCurrentQuestion();
    }

    public double ProgressValue => (double)(CurrentIndex + 1) / _questions.Count;
    public string ProgressText => $"{CurrentIndex + 1}/{_questions.Count}";
    public bool CanGoBack => CurrentIndex > 0;

    private void HookCurrentQuestion()
    {
        IsNextEnabled = !string.IsNullOrWhiteSpace(CurrentQuestion.UserInput);
        CurrentQuestion.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(FillInQuestion.UserInput))
            {
                IsNextEnabled = !string.IsNullOrWhiteSpace(CurrentQuestion.UserInput);
            }
        };
    }

    [RelayCommand(CanExecute = nameof(IsNextEnabled))]
    private async Task Next()
    {
        if (CurrentIndex < _questions.Count - 1)
        {
            CurrentIndex++;
            CurrentQuestion = _questions[CurrentIndex];
            HookCurrentQuestion();
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
        HookCurrentQuestion();
    }

    [RelayCommand]
    private void Restart()
    {
        foreach (var q in _questions)
            q.UserInput = string.Empty;

        ReviewResults.Clear();
        IsSummaryVisible = false;
        IsAnalyzing = false;
        CurrentIndex = 0;
        CurrentQuestion = _questions[0];
        HookCurrentQuestion();
    }

    private void BuildReview()
    {
        ReviewResults.Clear();
        int correctCount = 0;

        foreach (var q in _questions)
        {
            string trimmedUser = q.UserInput.Trim();
            bool isCorrect = string.Equals(trimmedUser, q.CorrectAnswer, StringComparison.OrdinalIgnoreCase);
            if (isCorrect) correctCount++;

            string sentence = $"{q.PromptBeforeBlank}[{q.CorrectAnswer}]{q.PromptAfterBlank}";
            ReviewResults.Add(new FillInReviewItem(
                sentence,
                string.IsNullOrEmpty(trimmedUser) ? "(Blank)" : trimmedUser,
                q.CorrectAnswer,
                isCorrect));
        }

        ScoreSummaryText = $"You filled in {correctCount} of {_questions.Count} blanks correctly";
    }
}