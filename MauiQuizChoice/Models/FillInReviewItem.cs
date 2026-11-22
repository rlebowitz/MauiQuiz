namespace MauiQuizChoice.Models;

public record FillInReviewItem(
    string FullSentence,
    string UserAnswer,
    string CorrectAnswer,
    bool IsCorrect);
