namespace MauiQuizChoice.Controls;

public class QuizNextButton : Button
{
    public QuizNextButton()
    {
        Text = "Next";
        HeightRequest = 52;
        CornerRadius = 26;
        FontSize = 16;
        FontAttributes = FontAttributes.Bold;
        TextColor = Colors.White;
        BackgroundColor = Color.FromArgb("#D1D5DB");
        Margin = new Thickness(24, 10, 24, 36);

        UpdateVisualState(IsEnabled);
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(IsEnabled))
        {
            UpdateVisualState(IsEnabled);
        }
    }

    private void UpdateVisualState(bool enabled)
    {
        BackgroundColor = enabled
            ? Color.FromArgb("#4FA8F6")
            : Color.FromArgb("#D1D5DB");
    }
}