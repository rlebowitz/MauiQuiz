using MauiQuizChoice.Resources.Strings;

namespace MauiQuizChoice.Models;

public class FlashcardReviewItem
{
    public string Prompt { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public bool Known { get; set; }
    public string StatusText => Known ? AppResources.MasteredStatusText : AppResources.NeedsReviewButtonText;
}