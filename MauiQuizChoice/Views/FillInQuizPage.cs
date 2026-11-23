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
                new Setter { Property = BackgroundColorProperty, Value = Color.FromArgb("#4FA8F6") }
            }
        });

        return new Grid
        {
            RowDefinitions = Rows.Define(265, Star, Auto),
            Children =
            {
                new CurvedHeaderBackground().Row(0),

                // Header Navigation + Progress + Sentence with Blank
                new VerticalStackLayout
                {
                    Padding = new Thickness(24, 14, 24, 24),
                    Spacing = 12,
                    Children =
                    {
                        new QuizTopNavBar
                        {
                            PreviousCommand = viewModel.PreviousCommand,
                            GoHomeCommand = viewModel.GoHomeCommand
                        }.Bind(QuizTopNavBar.CanGoBackProperty, static (FillInQuizViewModel vm) => vm.CanGoBack),

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

    private static AnalyzingOverlayView BuildAnalyzingOverlay() =>
        new AnalyzingOverlayView()
            .Bind(IsVisibleProperty, static (FillInQuizViewModel vm) => vm.IsAnalyzing);

    private static QuizSummaryView BuildSummaryView(FillInQuizViewModel viewModel) =>
        new QuizSummaryView {
            Title = "Fill-in-the-Blank Results",
            RetakeButtonText = "Retake Quiz",
            RetakeCommand = viewModel.RestartCommand,
            GoHomeCommand = viewModel.GoHomeCommand,
            ItemTemplate = new DataTemplate(CreateReviewItemCard)
        }
        .Bind(IsVisibleProperty, static (FillInQuizViewModel vm) => vm.IsSummaryVisible)
        .Bind(QuizSummaryView.SubtitleProperty, static (FillInQuizViewModel vm) => vm.ScoreSummaryText)
        .Bind(QuizSummaryView.ItemsSourceProperty, static (FillInQuizViewModel vm) => vm.ReviewResults);

    private static Grid CreateReviewItemCard() =>
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