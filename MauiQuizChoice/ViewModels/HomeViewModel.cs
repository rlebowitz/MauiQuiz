using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiQuizChoice.Views;
using System.Collections.ObjectModel;

namespace MauiQuizChoice.ViewModels;

public record QuizModeCard(
    string Title,
    string Subtitle,
    string QuestionCountBadge,
    string IconGlyph,
    string IconBackgroundHex,
    string IconTextHex,
    string TargetRoute);

public partial class HomeViewModel : ObservableObject
{
    public ObservableCollection<QuizModeCard> QuizModes { get; } =
    [
        new QuizModeCard(
            Title: "Multiple Choice",
            Subtitle: "Select the best answer from four options",
            QuestionCountBadge: "5 Questions",
            IconGlyph: "☰",
            IconBackgroundHex: "#EEF2FF",
            IconTextHex: "#6B66F6",
            TargetRoute: nameof(MultipleChoiceQuizPage)),

        new QuizModeCard(
            Title: "Matching Items",
            Subtitle: "Pair concepts with their matching definitions",
            QuestionCountBadge: "2 Rounds",
            IconGlyph: "⇄",
            IconBackgroundHex: "#EFF6FF",
            IconTextHex: "#4FA8F6",
            TargetRoute: nameof(MatchingQuizPage)),

        new QuizModeCard(
            Title: "True or False",
            Subtitle: "Decide whether each statement is accurate",
            QuestionCountBadge: "4 Statements",
            IconGlyph: "✓✕",
            IconBackgroundHex: "#F0FDF4",
            IconTextHex: "#16A34A",
            TargetRoute: nameof(TrueFalseQuizPage)),

        new QuizModeCard(
            Title: "Multi-Select",
            Subtitle: "Choose all answers that apply to each question",
            QuestionCountBadge: "3 Questions",
            IconGlyph: "☑",
            IconBackgroundHex: "#FEF3C7",
            IconTextHex: "#D97706",
            TargetRoute: nameof(MultiSelectQuizPage)),

        new QuizModeCard(
            Title: "Fill in the Blank",
            Subtitle: "Type the missing keyword to complete each statement",
            QuestionCountBadge: "3 Prompts",
            IconGlyph: "✎",
            IconBackgroundHex: "#FCE7F3",
            IconTextHex: "#DB2777",
            TargetRoute: nameof(FillInQuizPage)),
        new QuizModeCard(
            Title: "Flashcard Flip",
            Subtitle: "Flip cards to test your recall and self-assess",
            QuestionCountBadge: "3 CARDS",
            IconGlyph: "↻",
            IconBackgroundHex: "#FEF3C7",
            IconTextHex: "#D97706",
            TargetRoute: nameof(FlashcardQuizPage)),
        new QuizModeCard(
            Title: "Ordering & Sequencing",
            Subtitle: "Arrange scrambled steps into the proper order",
            QuestionCountBadge: "2 SEQUENCES",
            IconGlyph: "⇅",
            IconBackgroundHex: "#DCFCE7",
            IconTextHex: "#16A34A",
            TargetRoute: nameof(OrderingQuizPage))
    ];

    [RelayCommand]
    private static async Task LaunchQuiz(QuizModeCard? mode)
    {
        if (mode is null || string.IsNullOrWhiteSpace(mode.TargetRoute))
            return;

        await Shell.Current.GoToAsync(mode.TargetRoute);
    }
}