namespace MauiQuizChoice.Models;

public class TrueFalseReviewItem
{
    public string Statement { get; set; } = string.Empty;
    public string UserAnswerText { get; set; } = string.Empty;
    public string CorrectAnswerText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
