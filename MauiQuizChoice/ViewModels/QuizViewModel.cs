using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using MauiQuizChoice.Models;

namespace MauiQuizChoice.ViewModels;

public partial class QuizViewModel : ObservableObject
{
    private readonly List<Question> _questions;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressValue))]
    [NotifyPropertyChangedFor(nameof(ProgressText))]
    [NotifyPropertyChangedFor(nameof(CanGoBack))]
    [NotifyCanExecuteChangedFor(nameof(PreviousCommand))]
    public partial int CurrentIndex {get; set; }
    [ObservableProperty]
    public partial Question CurrentQuestion { get; set; }
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextCommand))]
    public partial bool IsNextEnabled {get; set; }
    [ObservableProperty]
    public partial bool IsAnalyzing {get; set; }
    [ObservableProperty]
    public partial bool IsSummaryVisible {get; set; }
    public ObservableCollection<SummaryItem> SummaryResults { get; } = [];

    // Circular progress properties
    public double ProgressValue => (double)(CurrentIndex + 1) / _questions.Count;
    public string ProgressText => $"{CurrentIndex + 1}/{_questions.Count}";
    public bool CanGoBack => CurrentIndex > 0;

    public QuizViewModel()
    {
        _questions =
        [
            new Question { Text = "My energy levels have been consistently low.", Options = CreateDefaultOptions() },
            new Question { Text = "Have you experienced feelings of sadness or emptiness?", Options = CreateDefaultOptions() },
            new Question { Text = "I find it difficult to concentrate on daily tasks.", Options = CreateDefaultOptions() },
            new Question { Text = "My sleep patterns have been disrupted recently.", Options = CreateDefaultOptions() },
            new Question { Text = "I feel overwhelmed by everyday responsibilities.", Options = CreateDefaultOptions() }
        ];
        CurrentQuestion = _questions[0];
    }

    private static ObservableCollection<AnswerOption> CreateDefaultOptions() =>
    [
        new AnswerOption { Text = "Not at all" },
        new AnswerOption { Text = "Occasionally" },
        new AnswerOption { Text = "Frequently" },
        new AnswerOption { Text = "Almost all the time" }
    ];

    [RelayCommand]
    private void SelectOption(AnswerOption selectedOption)
    {
        if (selectedOption == null) return;
        if (CurrentQuestion == null) return;
        foreach (var option in CurrentQuestion.Options)
        {
            option.IsSelected = (option == selectedOption);
        }

        IsNextEnabled = true;
    }

    [RelayCommand(CanExecute = nameof(IsNextEnabled))]
    private async Task Next()
    {
        if (CurrentIndex < _questions.Count - 1)
        {
            CurrentIndex++;
            CurrentQuestion = _questions[CurrentIndex];
            // Keep "Next" enabled if the user already answered this question earlier
            IsNextEnabled = CurrentQuestion.Options.Any(o => o.IsSelected);
        }
        else
        {
            // Show the "Analysing your answers" screen
            IsAnalyzing = true;
            await Task.Delay(3000); // Simulate processing
        
            BuildSummary();
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
        foreach (var question in _questions)
        {
            foreach (var option in question.Options)
            {
                option.IsSelected = false;
            }
        }

        SummaryResults.Clear();
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

    private void BuildSummary()
    {
        SummaryResults.Clear();
        foreach (var q in _questions)
        {
            var chosen = q.Options.FirstOrDefault(o => o.IsSelected)?.Text ?? "No answer";
            SummaryResults.Add(new SummaryItem(q.Text, chosen));
        }
    }

}
