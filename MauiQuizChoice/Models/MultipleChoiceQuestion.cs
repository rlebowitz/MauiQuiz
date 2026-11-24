using System.Collections.ObjectModel;

namespace MauiQuizChoice.Models;

public class MultipleChoiceQuestion
{
    public string Text { get; set; } = string.Empty;
    public ObservableCollection<MultipleChoiceAnswerOption> Options { get; set; } = [];
}
