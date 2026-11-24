using CommunityToolkit.Mvvm.Input;
using MauiQuizChoice.Models;
using System.Collections.ObjectModel;

namespace MauiQuizChoice.ViewModels;

public partial class SelectViewModel : QuizViewModelBase<MultipleChoiceQuestion, SummaryItem>
{
    public SelectViewModel() => InitializeQuiz();
    public override bool IsNextEnabled =>   CurrentItem.Options.Any(o => o.IsSelected) == true;
    protected override List<MultipleChoiceQuestion> CreateQuizItems()
    {
        return 
        [
            new MultipleChoiceQuestion { Text = "My energy levels have been consistently low.", Options = CreateDefaultOptions() },
            new MultipleChoiceQuestion { Text = "Have you experienced feelings of sadness or emptiness?", Options = CreateDefaultOptions() },
            new MultipleChoiceQuestion { Text = "I find it difficult to concentrate on daily tasks.", Options = CreateDefaultOptions() },
            new MultipleChoiceQuestion { Text = "My sleep patterns have been disrupted recently.", Options = CreateDefaultOptions() },
            new MultipleChoiceQuestion { Text = "I feel overwhelmed by everyday responsibilities.", Options = CreateDefaultOptions() }
        ];
    }
    private static ObservableCollection<MultipleChoiceAnswerOption> CreateDefaultOptions() =>
    [
        new MultipleChoiceAnswerOption { Text = "Not at all" },
        new MultipleChoiceAnswerOption { Text = "Occasionally" },
        new MultipleChoiceAnswerOption { Text = "Frequently" },
        new MultipleChoiceAnswerOption { Text = "Almost all the time" }
    ];

    [RelayCommand]
    private void SelectOption(MultipleChoiceAnswerOption selectedOption)
    {
        if (selectedOption == null) return;
        if (CurrentItem == null) return;
        foreach (var option in CurrentItem.Options)
        {
            option.IsSelected = (option == selectedOption);
        }

        NotifyQuizStateChanged();
    }

    protected override void EvaluateResults(IReadOnlyList<MultipleChoiceQuestion> items)
    {
        ReviewResults.Clear();
        foreach (var q in items)
        {
            var chosen = q.Options.FirstOrDefault(o => o.IsSelected)?.Text ?? "No selection";
            ReviewResults.Add(
                new SummaryItem{ QuestionText = q.Text, SelectedAnswer = chosen }
            );
        }
    }
  
    [RelayCommand]
    private static async Task CloseQuiz()
    {
        await Shell.Current.GoToAsync("..");
    }

}
