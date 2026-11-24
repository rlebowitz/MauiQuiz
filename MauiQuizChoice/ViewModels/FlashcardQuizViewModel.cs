using CommunityToolkit.Mvvm.Input;
using MauiQuizChoice.Models;

namespace MauiQuizChoice.ViewModels;

public partial class FlashcardQuizViewModel : QuizViewModelBase<FlashcardItem, FlashcardReviewItem>
{
    public FlashcardQuizViewModel() => InitializeQuiz();

    public override bool IsNextEnabled => CurrentItem?.Known is not null;

    protected override List<FlashcardItem> CreateQuizItems() =>
    [
        new()
        {
            Category = ".NET MAUI",
            FrontText = "What class serves as the base for all visual UI elements in .NET MAUI?",
            BackText = "VisualElement (inheriting from NavigableElement and Element)."
        },
        new()
        {
            Category = "C# Markup",
            FrontText = "Which method attaches a strongly-typed expression binding in CommunityToolkit.Maui.Markup?",
            BackText = "The .Bind() extension method using Expression<Func<TViewModel, TProperty>>."
        },
        new()
        {
            Category = "MVVM Toolkit",
            FrontText = "Which attribute generates an IRelayCommand from a method?",
            BackText = "[RelayCommand]"
        }
    ];

    [RelayCommand]
    private void MarkCard(bool known)
    {
        if (CurrentItem is null)
            return;

        CurrentItem.Known = known;
        NotifyQuizStateChanged();
    }

    protected override void EvaluateResults(IReadOnlyList<FlashcardItem> items)
    {
        ReviewResults.Clear();
        int masteredCount = 0;

        foreach (var card in items)
        {
            bool isKnown = card.Known == true;
            if (isKnown) masteredCount++;

            ReviewResults.Add(new FlashcardReviewItem
            {
                Prompt = card.FrontText,
                Answer = card.BackText,
                Known = isKnown
            });
        }

        ScoreSummaryText = $"You mastered {masteredCount} out of {items.Count} cards";
    }
}