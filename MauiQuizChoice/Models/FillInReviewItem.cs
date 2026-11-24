namespace MauiQuizChoice.Models;

public class FillInReviewItem
{
    public string FullSentence { get; set; } = string.Empty;
    public string UserAnswer { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
