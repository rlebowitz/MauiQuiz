using System.Collections.ObjectModel;

namespace MauiQuizChoice.Models;

public class Question
{
    public string Text { get; set; } = string.Empty;
    public ObservableCollection<AnswerOption> Options { get; set; } = [];
}
