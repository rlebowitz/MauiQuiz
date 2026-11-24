using System.Collections.ObjectModel;

namespace MauiQuizChoice.Models;

public class OrderingQuestion
{
    public string Prompt { get; set; } = string.Empty;
    public ObservableCollection<OrderStepItem> Steps { get; set; } = [];
}
