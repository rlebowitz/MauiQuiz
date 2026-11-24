using CommunityToolkit.Mvvm.Input;
using MauiQuizChoice.Models;

namespace MauiQuizChoice.ViewModels;

public partial class OrderingQuizViewModel : QuizViewModelBase<OrderingQuestion, OrderingReviewItem>
{
    public OrderingQuizViewModel() => InitializeQuiz();

    // Users can submit as soon as they are satisfied with the order
    public override bool IsNextEnabled => CurrentItem?.Steps.Count > 0;

    protected override IEnumerable<OrderingQuestion> CreateQuizItems()
    {
        var questions = new List<OrderingQuestion>
        {
            new()
            {
                Prompt = "Arrange the .NET MAUI app startup steps in order:",
                Steps =
                [
                    new() { Text = "Call builder.UseMauiApp<App>()", CorrectPosition = 2 },
                    new() { Text = "Create MauiApp.CreateBuilder()", CorrectPosition = 1 },
                    new() { Text = "Return builder.Build()", CorrectPosition = 4 },
                    new() { Text = "Register fonts, services, and handlers", CorrectPosition = 3 }
                ]
            },
            new()
            {
                Prompt = "Order the HTTP request lifecycle from client to response:",
                Steps =
                [
                    new() { Text = "Server processes request and returns status code", CorrectPosition = 3 },
                    new() { Text = "DNS resolves domain name to IP address", CorrectPosition = 1 },
                    new() { Text = "Client parses response body", CorrectPosition = 4 },
                    new() { Text = "TCP/TLS handshake establishes connection", CorrectPosition = 2 }
                ]
            }
        };

        foreach (var q in questions)
        {
            RenumberSteps(q);
        }

        return questions;
    }

    [RelayCommand]
    private void MoveUp(OrderStepItem step)
    {
        if (CurrentItem is null)
            return;

        int index = CurrentItem.Steps.IndexOf(step);
        if (index > 0)
        {
            CurrentItem.Steps.Move(index, index - 1);
            RenumberSteps(CurrentItem);
        }
    }

    [RelayCommand]
    private void MoveDown(OrderStepItem step)
    {
        if (CurrentItem is null)
            return;

        int index = CurrentItem.Steps.IndexOf(step);
        if (index >= 0 && index < CurrentItem.Steps.Count - 1)
        {
            CurrentItem.Steps.Move(index, index + 1);
            RenumberSteps(CurrentItem);
        }
    }

    public void OnDragReorderCompleted()
    {
        if (CurrentItem is not null)
        {
            RenumberSteps(CurrentItem);
        }
    }

    private static void RenumberSteps(OrderingQuestion question)
    {
        for (int i = 0; i < question.Steps.Count; i++)
        {
            question.Steps[i].DisplayNumber = i + 1;
        }
    }

    protected override void EvaluateResults(IReadOnlyList<OrderingQuestion> items)
    {
        ReviewResults.Clear();
        int exactCount = 0;

        foreach (var q in items)
        {
            bool isExact = true;
            for (int i = 0; i < q.Steps.Count; i++)
            {
                if (q.Steps[i].CorrectPosition != i + 1)
                {
                    isExact = false;
                    break;
                }
            }

            if (isExact) exactCount++;

            var userSequence = string.Join(" → ", q.Steps.Select(s => s.Text));
            var correctSequence = string.Join(" → ", q.Steps.OrderBy(s => s.CorrectPosition).Select(s => s.Text));

            ReviewResults.Add(new OrderingReviewItem
            {
                Prompt = q.Prompt,
                UserOrderText = userSequence,
                CorrectOrderText = correctSequence,
                IsCorrect = isExact
            });
        }

        ScoreSummaryText = $"You ordered {exactCount} out of {items.Count} sequences correctly";
    }
}