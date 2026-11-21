using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiQuizChoice.Models;
using System.Collections.ObjectModel;

namespace MauiQuizChoice.ViewModels;

public record TrueFalseReviewItem(string Statement, string UserAnswerText, string CorrectAnswerText, bool IsCorrect);

public partial class TrueFalseQuizViewModel : ObservableObject
{
    private readonly List<TrueFalseQuestion> _questions;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressValue))]
    [NotifyPropertyChangedFor(nameof(ProgressText))]
    [NotifyPropertyChangedFor(nameof(CanGoBack))]
    [NotifyCanExecuteChangedFor(nameof(PreviousCommand))]
    private int _currentIndex;

    [ObservableProperty]
    private TrueFalseQuestion _currentQuestion;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextCommand))]
    private bool _isNextEnabled;

    [ObservableProperty]
    private bool _isAnalyzing;

    [ObservableProperty]
    private bool _isSummaryVisible;

    [ObservableProperty]
    private string _scoreSummaryText = string.Empty;

    public ObservableCollection<TrueFalseReviewItem> ReviewResults { get; } = new();

    public TrueFalseQuizViewModel()
    {
        _questions = new List<TrueFalseQuestion>
        {
            new TrueFalseQuestion
            {
                Statement = "In .NET MAUI, TwoWay is the default binding mode for a Label's Text property.",
                CorrectAnswer = false
            },
            new TrueFalseQuestion
            {
                Statement = "GraphicsView requires calling Invalidate() to trigger a redraw when bindable properties change.",
                CorrectAnswer = true
            },
            new TrueFalseQuestion
            {
                Statement = "CommunityToolkit.Mvvm's [RelayCommand] attribute automatically appends 'Command' to the generated property name.",
                CorrectAnswer = true
            },
            new TrueFalseQuestion
            {
                Statement = "CollectionView creates visual elements for every single item in a 10,000-item list all at once.",
                CorrectAnswer = false
            }
        };

        _currentQuestion = _questions[0];
    }

    public double ProgressValue => (double)(CurrentIndex + 1) / _questions.Count;
    public string ProgressText => $"{CurrentIndex + 1}/{_questions.Count}";
    public bool CanGoBack => CurrentIndex > 0;

    [RelayCommand]
    private void SelectAnswer(string choice)
    {
        if (bool.TryParse(choice, out bool parsedChoice))
        {
            CurrentQuestion.UserAnswer = parsedChoice;
            IsNextEnabled = true;
        }
    }

    [RelayCommand(CanExecute = nameof(IsNextEnabled))]
    private async Task Next()
    {
        if (CurrentIndex < _questions.Count - 1)
        {
            CurrentIndex++;
            CurrentQuestion = _questions[CurrentIndex];
            IsNextEnabled = CurrentQuestion.UserAnswer.HasValue;
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
        IsNextEnabled = CurrentQuestion.UserAnswer.HasValue;
    }

    [RelayCommand]
    private void Restart()
    {
        foreach (var q in _questions)
        {
            q.UserAnswer = null;
        }

        ReviewResults.Clear();
        IsSummaryVisible = false;
        IsAnalyzing = false;
        CurrentIndex = 0;
        CurrentQuestion = _questions[0];
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

        foreach (var q in _questions)
        {
            bool isCorrect = q.UserAnswer == q.CorrectAnswer;
            if (isCorrect) correctCount++;

            string userText = q.UserAnswer.HasValue ? (q.UserAnswer.Value ? "True" : "False") : "Unanswered";
            string correctText = q.CorrectAnswer ? "True" : "False";

            ReviewResults.Add(new TrueFalseReviewItem(q.Statement, userText, correctText, isCorrect));
        }

        ScoreSummaryText = $"You got {correctCount} out of {_questions.Count} statements right";
    }
}