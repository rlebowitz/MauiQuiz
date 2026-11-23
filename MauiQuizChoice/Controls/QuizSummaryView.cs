using System.Collections;
using System.Windows.Input;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace MauiQuizChoice.Controls;

public partial class QuizSummaryView : Grid
{
    [BindableProperty]
    public partial string Title { get; set; } = "Quiz Results";
    [BindableProperty]
    public partial string Subtitle { get; set; } = string.Empty;
    [BindableProperty]
    public partial string RetakeButtonText { get; set; } = "Retake Quiz";

    [BindableProperty]
    public partial IEnumerable? ItemsSource { get; set; }

    [BindableProperty]
    public partial DataTemplate? ItemTemplate { get; set; }

    [BindableProperty]
    public partial ICommand? RetakeCommand { get; set; }

    [BindableProperty]
    public partial ICommand? GoHomeCommand { get; set; }    
  
    public QuizSummaryView()
    {
        RowDefinitions = Rows.Define(200, Star, Auto);
        BackgroundColor = Color.FromArgb("#F8F9FB");

        var titleLabel = new Label
        {
            TextColor = Colors.White,
            FontSize = 20,
            FontAttributes = FontAttributes.Bold
        }.TextCenterHorizontal();
        titleLabel.SetBinding(Label.TextProperty, new Binding(nameof(Title), source: this));

        var subtitleLabel = new Label
        {
            TextColor = Color.FromArgb("#E0E7FF"),
            FontSize = 14
        }.TextCenterHorizontal();
        subtitleLabel.SetBinding(Label.TextProperty, new Binding(nameof(Subtitle), source: this));

        var resultsCollection = new CollectionView
        {
            Margin = new Thickness(24, 12, 24, 0),
            SelectionMode = SelectionMode.None
        }.Row(1);
        resultsCollection.SetBinding(ItemsView.ItemsSourceProperty, new Binding(nameof(ItemsSource), source: this));
        resultsCollection.SetBinding(ItemsView.ItemTemplateProperty, new Binding(nameof(ItemTemplate), source: this));

        var retakeButton = new Button
        {
            BackgroundColor = Color.FromArgb("#4FA8F6"),
            TextColor = Colors.White,
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            HeightRequest = 52,
            CornerRadius = 26
        };
        retakeButton.SetBinding(Button.TextProperty, new Binding(nameof(RetakeButtonText), source: this));
        retakeButton.SetBinding(Button.CommandProperty, new Binding(nameof(RetakeCommand), source: this));

        var homeButton = new Button
        {
            Text = "Back to Home",
            BackgroundColor = Colors.Transparent,
            BorderColor = Color.FromArgb("#CBD5E1"),
            BorderWidth = 1.5,
            TextColor = Color.FromArgb("#475569"),
            FontSize = 15,
            FontAttributes = FontAttributes.Bold,
            HeightRequest = 48,
            CornerRadius = 24
        };
        homeButton.SetBinding(Button.CommandProperty, new Binding(nameof(GoHomeCommand), source: this));

        Children.Add(new CurvedHeaderBackground().Row(0));

        Children.Add(new VerticalStackLayout
        {
            Padding = new Thickness(24, 10, 24, 20),
            Spacing = 8,
            Children =
            {
                new Border
                {
                    BackgroundColor = Color.FromArgb("#5752D8"),
                    StrokeThickness = 0,
                    StrokeShape = new RoundRectangle { CornerRadius = 22 },
                    Content = new Label
                    {
                        Text = "✓",
                        TextColor = Colors.White,
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold
                    }.Center()
                }.Size(44).CenterHorizontal(),

                titleLabel,
                subtitleLabel
            }
        }.Row(0).CenterVertical());

        Children.Add(resultsCollection);

        Children.Add(new VerticalStackLayout
        {
            Spacing = 10,
            Margin = new Thickness(24, 10, 24, 32),
            Children = { retakeButton, homeButton }
        }.Row(2));
    }
}