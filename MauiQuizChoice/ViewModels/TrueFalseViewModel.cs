using CommunityToolkit.Mvvm.Input;
using MauiQuizChoice.Models;

namespace MauiQuizChoice.ViewModels;

public partial class TrueFalseQuizViewModel : QuizViewModelBase<TrueFalseQuestion, TrueFalseReviewItem>
{
    public TrueFalseQuizViewModel() => InitializeQuiz();

    protected override IEnumerable<TrueFalseQuestion> CreateQuizItems()
    {
        return
        [
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
        ];
    }
    public override bool IsNextEnabled =>
        CurrentItem is not null && (CurrentItem.IsTrueSelected || CurrentItem.IsFalseSelected);

    [RelayCommand]
    private void SelectAnswer(string choice)
    {
        bool isTrue = string.Equals(choice, "True", StringComparison.OrdinalIgnoreCase);
        CurrentItem.IsTrueSelected = isTrue;
        CurrentItem.IsFalseSelected = !isTrue;

        NotifyQuizStateChanged();
    }

    protected override void EvaluateResults(IReadOnlyList<TrueFalseQuestion> items)
    {
        ReviewResults.Clear();
        int correctCount = 0;

        foreach (var q in items)
        {
            bool userChoice = q.IsTrueSelected;
            bool isCorrect = userChoice == q.CorrectAnswer;
            if (isCorrect) correctCount++;
            var item = new TrueFalseReviewItem(
                q.Statement,
                userChoice ? "True" : "False",
                q.CorrectAnswer ? "True" : "False",
                isCorrect
            );
            ReviewResults.Add(item);
        }

        ScoreSummaryText = $"You scored {correctCount} out of {items.Count}";
    }

    [RelayCommand]
    private static async Task CloseQuiz()
    {
        await Shell.Current.GoToAsync("..");
    }
}