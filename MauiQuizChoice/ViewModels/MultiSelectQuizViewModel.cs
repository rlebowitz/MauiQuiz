using CommunityToolkit.Mvvm.Input;
using MauiQuizChoice.Models;
using System.Collections.ObjectModel;

namespace MauiQuizChoice.ViewModels;

public record MultiSelectReviewItem(
    string QuestionText,
    string UserSelectionsText,
    string CorrectSelectionsText,
    bool IsExactMatch);

public partial class MultiSelectQuizViewModel : QuizViewModelBase<MultiSelectQuestion, MultiSelectReviewItem>
{
    public MultiSelectQuizViewModel() => InitializeQuiz();
    public override bool IsNextEnabled =>
        CurrentItem?.Options.Any(o => o.IsSelected) == true;
    protected override IEnumerable<MultiSelectQuestion> CreateQuizItems()
    {
        return new List<MultiSelectQuestion>
        {
            new() {
                Text = "Which of the following are valid layout panels in .NET MAUI? (Select all that apply)",
                Options =
                [
                    new MultiSelectOption { Text = "Grid", IsCorrectOption = true },
                    new MultiSelectOption { Text = "FlexLayout", IsCorrectOption = true },
                    new MultiSelectOption { Text = "LinearLayout", IsCorrectOption = false },
                    new MultiSelectOption { Text = "VerticalStackLayout", IsCorrectOption = true }
                ]
            },
            new() {
                Text = "Which binding modes exist in .NET MAUI XAML?",
                Options =
                [
                    new MultiSelectOption { Text = "OneWay", IsCorrectOption = true },
                    new MultiSelectOption { Text = "TwoWay", IsCorrectOption = true },
                    new MultiSelectOption { Text = "OneTime", IsCorrectOption = true },
                    new MultiSelectOption { Text = "ReverseOnly", IsCorrectOption = false }
                ]
            },
            new() {
                Text = "Which attributes come from CommunityToolkit.Mvvm?",
                Options =
                [
                    new MultiSelectOption { Text = "[ObservableProperty]", IsCorrectOption = true },
                    new MultiSelectOption { Text = "[RelayCommand]", IsCorrectOption = true },
                    new MultiSelectOption { Text = "[InjectService]", IsCorrectOption = false },
                    new MultiSelectOption { Text = "[NotifyPropertyChangedFor]", IsCorrectOption = true }
                ]
            }
        };
    }

    [RelayCommand]
    private void ToggleOption(MultiSelectOption? option)
    {
        if (option is null) return;

        option.IsSelected = !option.IsSelected;
        NotifyQuizStateChanged();
    }

    protected override void EvaluateResults(IReadOnlyList<MultiSelectQuestion> items)
    {
        ReviewResults.Clear();
        int exactMatches = 0;

        foreach (var q in items)
        {
            var selected = q.Options.Where(o => o.IsSelected).Select(o => o.Text).ToList();
            var correct = q.Options.Where(o => o.IsCorrectOption).Select(o => o.Text).ToList();
            bool isExact = selected.Count == correct.Count && !selected.Except(correct).Any();
            if (isExact) exactMatches++;

            ReviewResults.Add(new MultiSelectReviewItem(q.Text, string.Join(", ", selected), string.Join(", ", correct),isExact));
        }

        ScoreSummaryText = $"You got {exactMatches} out of {items.Count} exact matches";
    }


}