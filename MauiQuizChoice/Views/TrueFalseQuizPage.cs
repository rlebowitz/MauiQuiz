using CommunityToolkit.Maui.Markup;
using MauiQuizChoice.Controls;
using MauiQuizChoice.ViewModels;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace MauiQuizChoice.Views;

public class TrueFalseQuizPage : ContentPage
{
    public TrueFalseQuizPage()
    {
        var viewModel = new TrueFalseQuizViewModel();
        BindingContext = viewModel;

        Shell.SetNavBarIsVisible(this, false);
        BackgroundColor = Color.FromArgb("#F8F9FB");

        Content = new Grid
        {
            Children =
            {
                BuildMainQuizView(viewModel),
                BuildAnalyzingOverlay(),
                BuildSummaryView(viewModel)
            }
        };
    }

    private static Grid BuildMainQuizView(TrueFalseQuizViewModel viewModel)
    {
        var nextButton = new Button
        {
            Text = "Next",
            Command = viewModel.NextCommand,
            HeightRequest = 52,
            CornerRadius = 26,
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            BackgroundColor = Color.FromArgb("#D1D5DB"),
            Margin = new Thickness(24, 10, 24, 36)
        }.Row(2);

        nextButton.Triggers.Add(new DataTrigger(typeof(Button))
        {
            Binding = new Binding(nameof(TrueFalseQuizViewModel.IsNextEnabled)),
            Value = true,
            Setters =
            {
                new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromArgb("#4FA8F6") }
            }
        });

        return new Grid
        {
            RowDefinitions = Rows.Define(260, Star, Auto),
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

                // Header Navigation + Progress + Statement
                new VerticalStackLayout
                {
                    Padding = new Thickness(24, 16, 24, 24),
                    Spacing = 14,
                    Children =
                    {
                        new Grid
                        {
                            ColumnDefinitions = Columns.Define(40, Star, 40),
                            HeightRequest = 36,
                            Children =
                            {
                                new Border
                                {
                                    BackgroundColor = Colors.Transparent,
                                    StrokeThickness = 0,
                                    GestureRecognizers = { new TapGestureRecognizer { Command = viewModel.PreviousCommand } },
                                    Content = new Label
                                    {
                                        Text = "←",
                                        TextColor = Colors.White,
                                        FontSize = 22
                                    }.Start().CenterVertical()
                                }
                                .Column(0)
                                .Bind(IsVisibleProperty, static (TrueFalseQuizViewModel vm) => vm.CanGoBack),

                                new Border
                                {
                                    BackgroundColor = Colors.Transparent,
                                    StrokeThickness = 0,
                                    GestureRecognizers = { new TapGestureRecognizer { Command = viewModel.RestartCommand } },
                                    Content = new Label
                                    {
                                        Text = "✕",
                                        TextColor = Colors.White,
                                        FontSize = 16
                                    }.End().CenterVertical()
                                }.Column(2)
                            }
                        },

                        new CircularProgressView()
                            .CenterHorizontal()
                            .Bind(CircularProgressView.ProgressProperty, static (TrueFalseQuizViewModel vm) => vm.ProgressValue)
                            .Bind(CircularProgressView.TextProperty, static (TrueFalseQuizViewModel vm) => vm.ProgressText),

                        new Label
                        {
                            TextColor = Colors.White,
                            FontSize = 18,
                            FontAttributes = FontAttributes.Bold,
                            LineBreakMode = LineBreakMode.WordWrap
                        }
                        .TextCenterHorizontal()
                        .Bind(Label.TextProperty, static (TrueFalseQuizViewModel vm) => vm.CurrentQuestion.Statement)
                    }
                }.Row(0),

                // Side-by-Side True / False Selection Cards
                new Grid
                {
                    ColumnDefinitions = Columns.Define(Star, Star),
                    ColumnSpacing = 16,
                    Margin = new Thickness(24, 0),
                    Children =
                    {
                        CreateChoiceCard(
                            viewModel: viewModel,
                            labelText: "True",
                            glyph: "✓",
                            glyphSize: 22,
                            commandParameter: "True",
                            selectedBindingPath: $"{nameof(TrueFalseQuizViewModel.CurrentQuestion)}.IsTrueSelected"
                        ).Column(0),

                        CreateChoiceCard(
                            viewModel: viewModel,
                            labelText: "False",
                            glyph: "✕",
                            glyphSize: 20,
                            commandParameter: "False",
                            selectedBindingPath: $"{nameof(TrueFalseQuizViewModel.CurrentQuestion)}.IsFalseSelected"
                        ).Column(1)
                    }
                }.Row(1).CenterVertical(),

                // Bottom "Next" Pill Button
                nextButton
            }
        };
    }

    private static Border CreateChoiceCard(
        TrueFalseQuizViewModel viewModel,
        string labelText,
        string glyph,
        double glyphSize,
        string commandParameter,
        string selectedBindingPath)
    {
        var iconLabel = new Label
        {
            Text = glyph,
            TextColor = Color.FromArgb("#64748B"),
            FontSize = glyphSize,
            FontAttributes = FontAttributes.Bold
        }.Center();

        iconLabel.Triggers.Add(new DataTrigger(typeof(Label))
        {
            Binding = new Binding(selectedBindingPath),
            Value = true,
            Setters =
            {
                new Setter { Property = Label.TextColorProperty, Value = Colors.White }
            }
        });

        var iconCircle = new Border
        {
            BackgroundColor = Color.FromArgb("#F1F5F9"),
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 26 },
            Content = iconLabel
        }.Size(52).CenterHorizontal();

        iconCircle.Triggers.Add(new DataTrigger(typeof(Border))
        {
            Binding = new Binding(selectedBindingPath),
            Value = true,
            Setters =
            {
                new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromArgb("#4FA8F6") }
            }
        });

        var card = new Border
        {
            BackgroundColor = Colors.White,
            Stroke = Color.FromArgb("#E2E8F0"),
            StrokeThickness = 1.5,
            HeightRequest = 170,
            Padding = 16,
            StrokeShape = new RoundRectangle { CornerRadius = 16 },
            GestureRecognizers =
            {
                new TapGestureRecognizer
                {
                    Command = viewModel.SelectAnswerCommand,
                    CommandParameter = commandParameter
                }
            },
            Content = new VerticalStackLayout
            {
                Spacing = 14,
                Children =
                {
                    iconCircle,
                    new Label
                    {
                        Text = labelText,
                        TextColor = Color.FromArgb("#2D3748"),
                        FontSize = 17,
                        FontAttributes = FontAttributes.Bold
                    }.CenterHorizontal()
                }
            }.Center()
        };

        card.Triggers.Add(new DataTrigger(typeof(Border))
        {
            Binding = new Binding(selectedBindingPath),
            Value = true,
            Setters =
            {
                new Setter { Property = Border.StrokeProperty, Value = Color.FromArgb("#4FA8F6") },
                new Setter { Property = Border.StrokeThicknessProperty, Value = 2.5 },
                new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromArgb("#EFF6FF") }
            }
        });

        return card;
    }

    private static Grid BuildAnalyzingOverlay() =>
        new Grid
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
        }.Bind(IsVisibleProperty, static (TrueFalseQuizViewModel vm) => vm.IsAnalyzing);

    private static Grid BuildSummaryView(TrueFalseQuizViewModel viewModel) =>
        new Grid
        {
            RowDefinitions = Rows.Define(200, Star, Auto),
            BackgroundColor = Color.FromArgb("#F8F9FB"),
            Children =
            {
                new Border
                {
                    BackgroundColor = Color.FromArgb("#6B66F6"),
                    StrokeThickness = 0,
                    Margin = new Thickness(-40, -50, -40, 0),
                    StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(0, 0, 180, 180) }
                }.Row(0),

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
                            Text = "True / False Results",
                            TextColor = Colors.White,
                            FontSize = 20,
                            FontAttributes = FontAttributes.Bold
                        }.TextCenterHorizontal(),

                        new Label
                        {
                            TextColor = Color.FromArgb("#E0E7FF"),
                            FontSize = 14
                        }
                        .TextCenterHorizontal()
                        .Bind(Label.TextProperty, static (TrueFalseQuizViewModel vm) => vm.ScoreSummaryText)
                    }
                }.Row(0).CenterVertical(),

                new CollectionView
                {
                    Margin = new Thickness(24, 12, 24, 0),
                    SelectionMode = SelectionMode.None,
                    ItemTemplate = new DataTemplate(CreateReviewItemCard)
                }
                .Row(1)
                .Bind(ItemsView.ItemsSourceProperty, static (TrueFalseQuizViewModel vm) => vm.ReviewResults),

                new Button
                {
                    Text = "Retake Quiz",
                    Command = viewModel.RestartCommand,
                    BackgroundColor = Color.FromArgb("#4FA8F6"),
                    TextColor = Colors.White,
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold,
                    HeightRequest = 52,
                    CornerRadius = 26,
                    Margin = new Thickness(24, 10, 24, 36)
                }.Row(2)
            }
        }.Bind(IsVisibleProperty, static (TrueFalseQuizViewModel vm) => vm.IsSummaryVisible);

    private static View CreateReviewItemCard() =>
        new Grid
        {
            Padding = new Thickness(0, 6),
            Children =
            {
                new Border
                {
                    BackgroundColor = Colors.White,
                    Stroke = Color.FromArgb("#E2E8F0"),
                    StrokeThickness = 1.5,
                    Padding = new Thickness(16, 14),
                    StrokeShape = new RoundRectangle { CornerRadius = 10 },
                    Content = new VerticalStackLayout
                    {
                        Spacing = 6,
                        Children =
                        {
                            new Label
                            {
                                TextColor = Color.FromArgb("#2D3748"),
                                FontSize = 14
                            }.Bind(Label.TextProperty, static (TrueFalseReviewItem item) => item.Statement),

                            new HorizontalStackLayout
                            {
                                Spacing = 12,
                                Children =
                                {
                                    new Label
                                    {
                                        TextColor = Color.FromArgb("#6B66F6"),
                                        FontSize = 13,
                                        FontAttributes = FontAttributes.Bold
                                    }.Bind(
                                        Label.TextProperty,
                                        static (TrueFalseReviewItem item) => item.UserAnswerText,
                                        stringFormat: "Your answer: {0}"),

                                    new Label
                                    {
                                        TextColor = Color.FromArgb("#16A34A"),
                                        FontSize = 13,
                                        FontAttributes = FontAttributes.Bold
                                    }
                                    .Bind(
                                        Label.TextProperty,
                                        static (TrueFalseReviewItem item) => item.CorrectAnswerText,
                                        stringFormat: "Correct: {0}")
                                    .Bind(
                                        IsVisibleProperty,
                                        static (TrueFalseReviewItem item) => item.IsCorrect,
                                        convert: static (bool isCorrect) => !isCorrect)
                                }
                            }
                        }
                    }
                }
            }
        };
}