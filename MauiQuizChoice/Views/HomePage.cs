using CommunityToolkit.Maui.Markup;
using MauiQuizChoice.ViewModels;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace MauiQuizChoice.Views;

public class HomePage : ContentPage
{
    public HomePage()
    {
        var viewModel = new HomeViewModel();
        BindingContext = viewModel;

        Shell.SetNavBarIsVisible(this, false);
        BackgroundColor = Color.FromArgb("#F8F9FB");

        Content = new Grid
        {
            RowDefinitions = Rows.Define(230, Star),
            Children =
            {
                // Curved Purple Header
                new Border
                {
                    BackgroundColor = Color.FromArgb("#6B66F6"),
                    StrokeThickness = 0,
                    Margin = new Thickness(-40, -50, -40, 0),
                    StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(0, 0, 180, 180) }
                }.Row(0),

                // Header Branding & Greeting
                new VerticalStackLayout
                {
                    Spacing = 10,
                    Padding = new Thickness(28, 16, 28, 24),
                    Children =
                    {
                        new Border
                        {
                            BackgroundColor = Color.FromArgb("#5752D8"),
                            StrokeThickness = 0,
                            StrokeShape = new RoundRectangle { CornerRadius = 24 },
                            Content = new Label
                            {
                                Text = "?",
                                TextColor = Colors.White,
                                FontSize = 22,
                                FontAttributes = FontAttributes.Bold
                            }.Center()
                        }.Size(48).CenterHorizontal(),

                        new Label
                        {
                            Text = "Knowledge Check",
                            TextColor = Colors.White,
                            FontSize = 24,
                            FontAttributes = FontAttributes.Bold
                        }.TextCenterHorizontal(),

                        new Label
                        {
                            Text = "Choose a quiz format below to begin",
                            TextColor = Color.FromArgb("#E0E7FF"),
                            FontSize = 14
                        }.TextCenterHorizontal()
                    }
                }.Row(0).CenterVertical(),

                // Quiz Mode Cards
                new CollectionView
                {
                    Margin = new Thickness(24, 14, 24, 24),
                    SelectionMode = SelectionMode.None,
                    ItemTemplate = new DataTemplate(() => CreateQuizModeCard(viewModel))
                }
                .Row(1)
                .Bind(ItemsView.ItemsSourceProperty, static (HomeViewModel vm) => vm.QuizModes)
            }
        };
    }

    private static View CreateQuizModeCard(HomeViewModel viewModel)
    {
        var tapGesture = new TapGestureRecognizer
        {
            Command = viewModel.LaunchQuizCommand
        };
        tapGesture.Bind(TapGestureRecognizer.CommandParameterProperty, ".");

        return new Grid
        {
            Padding = new Thickness(0, 8),
            Children =
            {
                new Border
                {
                    BackgroundColor = Colors.White,
                    Stroke = Color.FromArgb("#E2E8F0"),
                    StrokeThickness = 1.5,
                    Padding = 18,
                    StrokeShape = new RoundRectangle { CornerRadius = 14 },
                    GestureRecognizers = { tapGesture },
                    Content = new Grid
                    {
                        ColumnDefinitions = Columns.Define(Auto, Star, Auto),
                        ColumnSpacing = 14,
                        Children =
                        {
                            // Left Mode Icon Badge
                            new Border
                            {
                                StrokeThickness = 0,
                                StrokeShape = new RoundRectangle { CornerRadius = 12 },
                                Content = new Label
                                {
                                    FontSize = 18,
                                    FontAttributes = FontAttributes.Bold
                                }
                                .Center()
                                .Bind(Label.TextProperty, static (QuizModeCard c) => c.IconGlyph)
                                .Bind(
                                    Label.TextColorProperty,
                                    static (QuizModeCard c) => c.IconTextHex,
                                    convert: static (string? hex) => string.IsNullOrWhiteSpace(hex) ? Colors.Transparent : Color.FromArgb(hex))
                            }
                            .Column(0)
                            .Size(48)
                            .CenterVertical()
                            .Bind(
                                BackgroundColorProperty,
                                static (QuizModeCard c) => c.IconBackgroundHex,
                                convert: static (string? hex) => string.IsNullOrWhiteSpace(hex) ? Colors.Transparent : Color.FromArgb(hex)),

                            // Title, Subtitle & Question Count Pill
                            new VerticalStackLayout
                            {
                                Spacing = 4,
                                Children =
                                {
                                    new Label
                                    {
                                        TextColor = Color.FromArgb("#2D3748"),
                                        FontSize = 16,
                                        FontAttributes = FontAttributes.Bold
                                    }.Bind(Label.TextProperty, static (QuizModeCard c) => c.Title),

                                    new Label
                                    {
                                        TextColor = Color.FromArgb("#64748B"),
                                        FontSize = 13,
                                        LineBreakMode = LineBreakMode.WordWrap
                                    }.Bind(Label.TextProperty, static (QuizModeCard c) => c.Subtitle),

                                    new Border
                                    {
                                        BackgroundColor = Color.FromArgb("#F1F5F9"),
                                        StrokeThickness = 0,
                                        Padding = new Thickness(8, 3),
                                        Margin = new Thickness(0, 4, 0, 0),
                                        StrokeShape = new RoundRectangle { CornerRadius = 6 },
                                        Content = new Label
                                        {
                                            TextColor = Color.FromArgb("#475569"),
                                            FontSize = 11,
                                            FontAttributes = FontAttributes.Bold
                                        }.Bind(Label.TextProperty, static (QuizModeCard c) => c.QuestionCountBadge)
                                    }.Start()
                                }
                            }.Column(1).CenterVertical(),

                            // Right Launch Chevron
                            new Label
                            {
                                Text = "›",
                                TextColor = Color.FromArgb("#4FA8F6"),
                                FontSize = 26,
                                FontAttributes = FontAttributes.Bold
                            }.Column(2).CenterVertical()
                        }
                    }
                }
            }
        };
    }
}