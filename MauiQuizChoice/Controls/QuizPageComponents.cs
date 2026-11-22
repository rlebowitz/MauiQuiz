using System.Collections;
using System.Windows.Input;
using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace MauiQuizChoice.Controls;

public static class QuizPageComponents
{
    /// <summary>
    /// Creates the curved purple header background shared across all quiz and summary screens.
    /// </summary>
    public static Border CreateCurvedHeaderBackground(int row = 0) =>
        new Border
        {
            BackgroundColor = Color.FromArgb("#6B66F6"),
            StrokeThickness = 0,
            Margin = new Thickness(-40, -50, -40, 0),
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(0, 0, 180, 180) }
        }.Row(row);

    /// <summary>
    /// Creates the top navigation bar with a conditional back arrow (←) and a close/home button (✕).
    /// </summary>
    public static Grid CreateTopNavBar<TViewModel>(
        ICommand previousCommand,
        ICommand goHomeCommand,
        Func<TViewModel, bool> canGoBackSelector)
    {
        return new Grid
        {
            ColumnDefinitions = Columns.Define(40, Star, 40),
            HeightRequest = 36,
            Children =
            {
                new Border
                {
                    BackgroundColor = Colors.Transparent,
                    StrokeThickness = 0,
                    GestureRecognizers = { new TapGestureRecognizer { Command = previousCommand } },
                    Content = new Label
                    {
                        Text = "←",
                        TextColor = Colors.White,
                        FontSize = 22
                    }.Start().CenterVertical()
                }
                .Column(0)
                .Bind(VisualElement.IsVisibleProperty, canGoBackSelector),

                new Border
                {
                    BackgroundColor = Colors.Transparent,
                    StrokeThickness = 0,
                    GestureRecognizers = { new TapGestureRecognizer { Command = goHomeCommand } },
                    Content = new Label
                    {
                        Text = "✕",
                        TextColor = Colors.White,
                        FontSize = 16
                    }.End().CenterVertical()
                }.Column(2)
            }
        };
    }

    /// <summary>
    /// Creates the circular progress ring bound to ProgressValue and ProgressText.
    /// </summary>
    public static CircularProgressView CreateProgressRing<TViewModel>(Func<TViewModel, double> progressValueSelector, Func<TViewModel, string> progressTextSelector)
    {
        return new CircularProgressView()
            .CenterHorizontal()
            .Bind(CircularProgressView.ProgressProperty, progressValueSelector)
            .Bind(CircularProgressView.TextProperty, progressTextSelector);
    }

    /// <summary>
    /// Creates the full-screen purple "Analysing your answers" overlay.
    /// </summary>
    public static Grid CreateAnalyzingOverlay<TViewModel>(
        Func<TViewModel, bool> isAnalyzingSelector)
    {
        return new Grid
        {
            BackgroundColor = Color.FromArgb("#6B66F6"),
            Children =
            {
                new VerticalStackLayout
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

                        new Label
                        {
                            Text = "Analysing your answers",
                            TextColor = Colors.White,
                            FontSize = 18,
                            FontAttributes = FontAttributes.Bold
                        }.CenterHorizontal()
                    }
                }.Center()
            }
        }.Bind(VisualElement.IsVisibleProperty, isAnalyzingSelector);
    }

    /// <summary>
    /// Creates the standardized results summary screen with a retake button and a return-home button.
    /// </summary>
    public static Grid CreateSummaryView<TViewModel, TItemsSource>(
        string headerTitle,
        string retakeButtonText,
        ICommand restartCommand,
        ICommand goHomeCommand,
        Func<TViewModel, bool> isSummaryVisibleSelector,
        Func<TViewModel, TItemsSource> resultsSourceSelector,
        Func<View> itemTemplateFactory,
        Func<TViewModel, string>? scoreSummarySelector = null,
        string? staticSubtitle = null)
        where TItemsSource : IEnumerable
    {
        var subtitleLabel = new Label
        {
            TextColor = Color.FromArgb("#E0E7FF"),
            FontSize = 14
        }.TextCenterHorizontal();

        if (scoreSummarySelector is not null)
        {
            subtitleLabel.Bind(Label.TextProperty, scoreSummarySelector);
        }
        else if (!string.IsNullOrEmpty(staticSubtitle))
        {
            subtitleLabel.Text = staticSubtitle;
        }

        return new Grid
        {
            RowDefinitions = Rows.Define(200, Star, Auto),
            BackgroundColor = Color.FromArgb("#F8F9FB"),
            Children =
            {
                CreateCurvedHeaderBackground(row: 0),

                new VerticalStackLayout
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

                        new Label
                        {
                            Text = headerTitle,
                            TextColor = Colors.White,
                            FontSize = 20,
                            FontAttributes = FontAttributes.Bold
                        }.TextCenterHorizontal(),

                        subtitleLabel
                    }
                }.Row(0).CenterVertical(),

                new CollectionView
                {
                    Margin = new Thickness(24, 12, 24, 0),
                    SelectionMode = SelectionMode.None,
                    ItemTemplate = new DataTemplate(itemTemplateFactory)
                }
                .Row(1)
                .Bind(ItemsView.ItemsSourceProperty, resultsSourceSelector),

                new VerticalStackLayout
                {
                    Spacing = 10,
                    Margin = new Thickness(24, 10, 24, 32),
                    Children =
                    {
                        new Button
                        {
                            Text = retakeButtonText,
                            Command = restartCommand,
                            BackgroundColor = Color.FromArgb("#4FA8F6"),
                            TextColor = Colors.White,
                            FontSize = 16,
                            FontAttributes = FontAttributes.Bold,
                            HeightRequest = 52,
                            CornerRadius = 26
                        },
                        new Button
                        {
                            Text = "Back to Home",
                            Command = goHomeCommand,
                            BackgroundColor = Colors.Transparent,
                            BorderColor = Color.FromArgb("#CBD5E1"),
                            BorderWidth = 1.5,
                            TextColor = Color.FromArgb("#475569"),
                            FontSize = 15,
                            FontAttributes = FontAttributes.Bold,
                            HeightRequest = 48,
                            CornerRadius = 24
                        }
                    }
                }.Row(2)
            }
        }.Bind(VisualElement.IsVisibleProperty, isSummaryVisibleSelector);
    }

    /// <summary>
    /// Creates the bottom "Next" pill button with an automatic DataTrigger for IsNextEnabled.
    /// </summary>
    public static Button CreateNextButton(ICommand nextCommand, string isNextEnabledPropertyName, int row = 2)
    {
        var button = new Button
        {
            Text = "Next",
            Command = nextCommand,
            HeightRequest = 52,
            CornerRadius = 26,
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            BackgroundColor = Color.FromArgb("#D1D5DB"),
            Margin = new Thickness(24, 10, 24, 36)
        }.Row(row);

        button.Triggers.Add(new DataTrigger(typeof(Button))
        {
            Binding = new Binding(isNextEnabledPropertyName),
            Value = true,
            Setters =
            {
                new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromArgb("#4FA8F6") }
            }
        });

        return button;
    }
}