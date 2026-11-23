using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;

namespace MauiQuizChoice.Controls;

public class AnalyzingOverlayView : Grid
{
    public static readonly BindableProperty MessageProperty =
        BindableProperty.Create(nameof(Message), typeof(string), typeof(AnalyzingOverlayView), "Analysing your answers");

    public string Message
    {
        get => (string)GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public AnalyzingOverlayView()
    {
        BackgroundColor = Color.FromArgb("#6B66F6");

        var messageLabel = new Label
        {
            TextColor = Colors.White,
            FontSize = 18,
            FontAttributes = FontAttributes.Bold
        }.CenterHorizontal();
        messageLabel.SetBinding(Label.TextProperty, new Binding(nameof(Message), source: this));

        Children.Add(new VerticalStackLayout
        {
            Spacing = 24,
            Children =
            {
                new Border
                {
                    BackgroundColor = Color.FromArgb("#C7D2FE"),
                    StrokeThickness = 0,
                    Padding = 14,
                    StrokeShape = new RoundRectangle { CornerRadius = 8 },
                    Content = new VerticalStackLayout
                    {
                        Spacing = 10,
                        Children =
                        {
                            new BoxView { HeightRequest = 8, WidthRequest = 32, CornerRadius = 4, Color = Colors.White }.Start(),
                            new BoxView { HeightRequest = 8, WidthRequest = 54, CornerRadius = 4, Color = Colors.White }.Start(),
                            new BoxView { HeightRequest = 8, WidthRequest = 44, CornerRadius = 4, Color = Colors.White }.Start(),
                            new BoxView { HeightRequest = 8, WidthRequest = 50, CornerRadius = 4, Color = Color.FromArgb("#818CF8") }.Start()
                        }
                    }.CenterVertical()
                }.Size(84, 105).CenterHorizontal(),

                messageLabel
            }
        }.Center());
    }
}