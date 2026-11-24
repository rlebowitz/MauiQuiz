namespace MauiQuizChoice.Models;

public class MatchReviewItem
{
    public string Prompt { get; set; } = string.Empty;
    public string UserMatch { get; set; } = string.Empty;
    public string CorrectMatch { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
