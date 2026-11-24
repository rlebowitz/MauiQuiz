namespace MauiQuizChoice.Models;

public class OrderingReviewItem
{
    public string Prompt { get; set; } = string.Empty;
    public string UserOrderText { get; set; } = string.Empty;
    public string CorrectOrderText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}