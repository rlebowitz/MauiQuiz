using System.Collections;
using System.Windows.Input;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace MauiQuizChoice.Controls;

public class QuizSummaryView : Grid
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(QuizSummaryView), "Quiz Results");

    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(QuizSummaryView), string.Empty);

    public static readonly BindableProperty RetakeButtonTextProperty =
        BindableProperty.Create(nameof(RetakeButtonText), typeof(string), typeof(QuizSummaryView), "Retake Quiz");

    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(QuizSummaryView));

    public static readonly BindableProperty ItemTemplateProperty =
        BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(QuizSummaryView));

    public static readonly BindableProperty RetakeCommandProperty =
        BindableProperty.Create(nameof(RetakeCommand), typeof(ICommand), typeof(QuizSummaryView));

    public static readonly BindableProperty GoHomeCommandProperty =
        BindableProperty.Create(nameof(GoHomeCommand), typeof(ICommand), typeof(QuizSummaryView));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public string RetakeButtonText
    {
        get => (string)GetValue(RetakeButtonTextProperty);
        set => SetValue(RetakeButtonTextProperty, value);
    }

    public IEnumerable? ItemsSource
    {
        get => (IEnumerable?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public DataTemplate? ItemTemplate
    {
        get => (DataTemplate?)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public ICommand? RetakeCommand
    {
        get => (ICommand?)GetValue(RetakeCommandProperty);
        set => SetValue(RetakeCommandProperty, value);
    }

    public ICommand? GoHomeCommand
    {
        get => (ICommand?)GetValue(GoHomeCommandProperty);
        set => SetValue(GoHomeCommandProperty, value);
    }

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