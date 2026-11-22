using CommunityToolkit.Mvvm.Input;
using MauiQuizChoice.Models;

namespace MauiQuizChoice.ViewModels;

public partial class MatchingQuizViewModel : QuizViewModelBase<MatchingRound, MatchReviewItem>
{
    public MatchingQuizViewModel() => InitializeQuiz();
    public override bool IsNextEnabled =>
             CurrentItem?.Prompts.All(p => p.IsMatched) == true;
    protected override IEnumerable<MatchingRound> CreateQuizItems()
    {
        return
        [
            new MatchingRound
            {
                Instruction = "Match each .NET MAUI layout with its primary behavior.",
                Prompts =
                [
                    new MatchPromptItem { PairId = 1, Text = "Grid" },
                    new MatchPromptItem { PairId = 2, Text = "VerticalStackLayout" },
                    new MatchPromptItem { PairId = 3, Text = "CollectionView" }
                ],
                Targets =
                [
                    new MatchTargetItem { PairId = 2, Text = "Stacks child views in a single column" },
                    new MatchTargetItem { PairId = 3, Text = "Renders virtualized scrollable lists" },
                    new MatchTargetItem { PairId = 1, Text = "Arranges views into rows and columns" }
                ]
            },
            new MatchingRound
            {
                Instruction = "Match each MVVM attribute with its generated output.",
                Prompts =
                [
                    new MatchPromptItem { PairId = 4, Text = "[ObservableProperty]" },
                    new MatchPromptItem { PairId = 5, Text = "[RelayCommand]" },
                    new MatchPromptItem { PairId = 6, Text = "[BindableProperty]" }
                ],
                Targets =
                [
                    new MatchTargetItem { PairId = 6, Text = "Creates XAML-bindable control properties" },
                    new MatchTargetItem { PairId = 4, Text = "Generates INotifyPropertyChanged boilerplate" },
                    new MatchTargetItem { PairId = 5, Text = "Wraps a method into an ICommand" }
                ]
            }
        ];
    }

    [RelayCommand]
    private void SelectPrompt(MatchPromptItem prompt)
    {
        NotifyQuizStateChanged();
    }

    [RelayCommand]
    private void SelectTarget(MatchTargetItem target)
    {
        NotifyQuizStateChanged();
    }

    protected override void EvaluateResults(IReadOnlyList<MatchingRound> items)
    {
        ReviewResults.Clear();
        int correctPairs = 0;
        int totalPairs = 0;

        foreach (var round in items)
        {
            for (int i = 0; i < round.Prompts.Count; i++)
            {
                totalPairs++;
                var prompt = round.Prompts[i];
                var target = round.Targets[i];
                bool isCorrect = prompt.PairId == target.PairId;
                if (isCorrect) correctPairs++;

                var x = new MatchReviewItem(
                     prompt.Text,
                     prompt.MatchedTargetText ?? "Unmatched",
                     target.MatchedBadgeText,
                     isCorrect
                 );
                ReviewResults.Add(x);
            }
        }

        ScoreSummaryText = $"You matched {correctPairs} out of {totalPairs} pairs";
    }
}