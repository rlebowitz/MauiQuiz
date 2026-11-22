using CommunityToolkit.Maui.Markup;
using MauiQuizChoice.Controls;
using MauiQuizChoice.Models;
using MauiQuizChoice.ViewModels;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace MauiQuizChoice.Views;

public class FillInQuizPage : ContentPage
{
    public FillInQuizPage()
    {
        var viewModel = new FillInQuizViewModel();
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

    private static Grid BuildMainQuizView(FillInQuizViewModel viewModel)
    {
        // Input border with DataTrigger equivalent
        var inputBorder = new Border
        {
            BackgroundColor = Colors.White,
            Stroke = Color.FromArgb("#E2E8F0"),
            StrokeThickness = 1.5,
            Padding = new Thickness(16, 10),
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Content = new Entry
            {
                Placeholder = "Type your answer here...",
                PlaceholderColor = Color.FromArgb("#94A3B8"),
                TextColor = Color.FromArgb("#2D3748"),
                FontSize = 16,
                ClearButtonVisibility = ClearButtonVisibility.WhileEditing,
                ReturnType = ReturnType.Next,
                ReturnCommand = viewModel.NextCommand
            }.Bind(Entry.TextProperty, static (FillInQuizViewModel vm) => vm.CurrentItem.UserInput)
        };

        inputBorder.Triggers.Add(new DataTrigger(typeof(Border))
        {
            Binding = new Binding(nameof(FillInQuizViewModel.IsNextEnabled)),
            Value = true,
            Setters =
            {
                new Setter { Property = Border.StrokeProperty, Value = Color.FromArgb("#4FA8F6") },
                new Setter { Property = Border.StrokeThicknessProperty, Value = 2.0 }
            }
        });

        // Next button with DataTrigger equivalent
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
            Binding = new Binding(nameof(FillInQuizViewModel.IsNextEnabled)),
            Value = true,
            Setters =
            {
                new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromArgb("#4FA8F6") }
            }
        });

        return new Grid
        {
            RowDefinitions = Rows.Define(265, Star, Auto),
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

                // Header Navigation + Progress + Sentence with Blank
                new VerticalStackLayout
                {
                    Padding = new Thickness(24, 14, 24, 24),
                    Spacing = 12,
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
                                .Bind(IsVisibleProperty, static (FillInQuizViewModel vm) => vm.CanGoBack),

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
                            .Bind(CircularProgressView.ProgressProperty, static (FillInQuizViewModel vm) => vm.ProgressValue)
                            .Bind(CircularProgressView.TextProperty, static (FillInQuizViewModel vm) => vm.ProgressText),

                        // Formatted Sentence with Inline Blank
                        new Label
                        {
                            LineBreakMode = LineBreakMode.WordWrap,
                            FontSize = 17,
                            FormattedText = new FormattedString
                            {
                                Spans =
                                {
                                    new Span
                                    {
                                        TextColor = Colors.White,
                                        FontAttributes = FontAttributes.Bold
                                    }.Bind(Span.TextProperty, static (FillInQuizViewModel vm) => vm.CurrentItem.PromptBeforeBlank),

                                    new Span
                                    {
                                        Text = "  ______  ",
                                        TextColor = Color.FromArgb("#93C5FD"),
                                        FontAttributes = FontAttributes.Bold
                                    },

                                    new Span
                                    {
                                        TextColor = Colors.White,
                                        FontAttributes = FontAttributes.Bold
                                    }.Bind(Span.TextProperty, static (FillInQuizViewModel vm) => vm.CurrentItem.PromptAfterBlank)
                                }
                            }
                        }.TextCenterHorizontal()
                    }
                }.Row(0),

                // Answer Input Card & Hint Pill
                new VerticalStackLayout
                {
                    Padding = new Thickness(24, 24, 24, 0),
                    Spacing = 14,
                    Children =
                    {
                        inputBorder,

                        new Border
                        {
                            BackgroundColor = Color.FromArgb("#EEF2FF"),
                            StrokeThickness = 0,
                            Padding = new Thickness(12, 8),
                            StrokeShape = new RoundRectangle { CornerRadius = 8 },
                            Content = new Label
                            {
                                TextColor = Color.FromArgb("#4F46E5"),
                                FontSize = 12,
                                FontAttributes = FontAttributes.Bold
                            }.Bind(
                                Label.TextProperty,
                                static (FillInQuizViewModel vm) => vm.CurrentItem.Hint,
                                stringFormat: "Hint: {0}")
                        }.Start()
                    }
                }.Row(1).Top(),

                // Bottom "Next" Pill Button
                nextButton
            }
        };
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
        }.Bind(IsVisibleProperty, static (FillInQuizViewModel vm) => vm.IsAnalyzing);

    private static Grid BuildSummaryView(FillInQuizViewModel viewModel) =>
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
                            Text = "Fill-In Results",
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
                        .Bind(Label.TextProperty, static (FillInQuizViewModel vm) => vm.ScoreSummaryText)
                    }
                }.Row(0).CenterVertical(),

                new CollectionView
                {
                    Margin = new Thickness(24, 12, 24, 0),
                    SelectionMode = SelectionMode.None,
                    ItemTemplate = new DataTemplate(CreateReviewItemCard)
                }
                .Row(1)
                .Bind(ItemsView.ItemsSourceProperty, static (FillInQuizViewModel vm) => vm.ReviewResults),

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
        }.Bind(IsVisibleProperty, static (FillInQuizViewModel vm) => vm.IsSummaryVisible);

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
                        Spacing = 4,
                        Children =
                        {
                            new Label
                            {
                                TextColor = Color.FromArgb("#2D3748"),
                                FontSize = 14,
                                FontAttributes = FontAttributes.Bold
                            }.Bind(Label.TextProperty, static (FillInReviewItem item) => item.FullSentence),

                            new Label
                            {
                                TextColor = Color.FromArgb("#6B66F6"),
                                FontSize = 13
                            }.Bind(
                                Label.TextProperty,
                                static (FillInReviewItem item) => item.UserAnswer,
                                stringFormat: "Your answer: {0}"),

                            new Label
                            {
                                TextColor = Color.FromArgb("#16A34A"),
                                FontSize = 13
                            }
                            .Bind(
                                Label.TextProperty,
                                static (FillInReviewItem item) => item.CorrectAnswer,
                                stringFormat: "Expected: {0}")
                            .Bind(
                                IsVisibleProperty,
                                static (FillInReviewItem item) => item.IsCorrect,
                                convert: static (bool isCorrect) => !isCorrect)
                        }
                    }
                }
            }
        };
}