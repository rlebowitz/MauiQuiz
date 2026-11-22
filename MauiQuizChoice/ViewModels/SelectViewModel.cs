using CommunityToolkit.Mvvm.Input;
using MauiQuizChoice.Models;
using System.Collections.ObjectModel;

namespace MauiQuizChoice.ViewModels;

public partial class SelectViewModel : QuizViewModelBase<Question, SummaryItem>
{
    public SelectViewModel() => InitializeQuiz();
    public override bool IsNextEnabled =>   CurrentItem.Options.Any(o => o.IsSelected) == true;
    protected override IEnumerable<Question> CreateQuizItems()
    {
        return 
        [
            new Question { Text = "My energy levels have been consistently low.", Options = CreateDefaultOptions() },
            new Question { Text = "Have you experienced feelings of sadness or emptiness?", Options = CreateDefaultOptions() },
            new Question { Text = "I find it difficult to concentrate on daily tasks.", Options = CreateDefaultOptions() },
            new Question { Text = "My sleep patterns have been disrupted recently.", Options = CreateDefaultOptions() },
            new Question { Text = "I feel overwhelmed by everyday responsibilities.", Options = CreateDefaultOptions() }
        ];
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
        if (CurrentItem == null) return;
        foreach (var option in CurrentItem.Options)
        {
            option.IsSelected = (option == selectedOption);
        }

        NotifyQuizStateChanged();
    }

    protected override void EvaluateResults(IReadOnlyList<Question> items)
    {
        ReviewResults.Clear();
        foreach (var q in items)
        {
            var chosen = q.Options.FirstOrDefault(o => o.IsSelected)?.Text ?? "No selection";
            ReviewResults.Add(new SummaryItem(q.Text, chosen));
        }
    }
  
    [RelayCommand]
    private static async Task CloseQuiz()
    {
        await Shell.Current.GoToAsync("..");
    }

}
