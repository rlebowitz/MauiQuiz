using MauiQuizChoice.Models;

namespace MauiQuizChoice.ViewModels;

public partial class FillInQuizViewModel : QuizViewModelBase<FillInQuestion, FillInReviewItem>
{
    public FillInQuizViewModel() => InitializeQuiz();

    public override bool IsNextEnabled =>
        !string.IsNullOrWhiteSpace(CurrentItem?.UserInput);

    protected override List<FillInQuestion> CreateQuizItems()
    {
        return
        [
            new() {
                PromptBeforeBlank = "In .NET MAUI, a GraphicsView requires calling ",
                PromptAfterBlank = "() to trigger a canvas redraw.",
                CorrectAnswer = "Invalidate",
                Hint = "Method on IGraphicsView"
            },
            new()
            {
                PromptBeforeBlank = "The ",
                PromptAfterBlank = " attribute generates ICommand properties from methods.",
                CorrectAnswer = "RelayCommand",
                Hint = "CommunityToolkit.Mvvm attribute"
            },
            new() {
                PromptBeforeBlank = "Use ",
                PromptAfterBlank = " mode when UI edits must update the ViewModel property.",
                CorrectAnswer = "TwoWay",
                Hint = "Default for Entry.Text"
            }
        ];
    }

    protected override void EvaluateResults(IReadOnlyList<FillInQuestion> items)
    {
        ReviewResults.Clear();
        int correctCount = 0;

        foreach (var q in items)
        {
            var trimmed = q.UserInput?.Trim() ?? string.Empty;
            bool isCorrect = string.Equals(trimmed, q.CorrectAnswer, StringComparison.OrdinalIgnoreCase);
            if (isCorrect) correctCount++;

            ReviewResults.Add(new FillInReviewItem
            (
                $"{q.PromptBeforeBlank} {q.CorrectAnswer} {q.PromptAfterBlank}",
                string.IsNullOrEmpty(trimmed) ? "(blank)" : trimmed,
                q.CorrectAnswer,
                isCorrect
            ));
        }

        ScoreSummaryText = $"You scored {correctCount} out of {items.Count}";
    }
}