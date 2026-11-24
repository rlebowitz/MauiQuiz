using System.Globalization;
using CommunityToolkit.Maui.Markup;
using MauiQuizChoice.Controls;
using MauiQuizChoice.Models;
using MauiQuizChoice.Resources.Strings;
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

        // Automatically mirror layout for Hebrew (RTL) vs. English (LTR)
        FlowDirection = CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft
            ? FlowDirection.RightToLeft
            : FlowDirection.LeftToRight;

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
        // Input border with strongly-typed expression bindings
        var inputBorder = new Border
        {
            BackgroundColor = Colors.White,
            Padding = new Thickness(16, 10),
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Content = new Entry
            {
                Placeholder = AppResources.FillInAnswerPlaceholder,
                PlaceholderColor = Color.FromArgb("#94A3B8"),
                TextColor = Color.FromArgb("#2D3748"),
                FontSize = 16,
                ClearButtonVisibility = ClearButtonVisibility.WhileEditing,
                ReturnType = ReturnType.Next,
                ReturnCommand = viewModel.NextCommand
            }.Bind(Entry.TextProperty, static (FillInQuizViewModel vm) => vm.CurrentItem.UserInput)
        }
        .Bind<Border, FillInQuizViewModel, bool, Brush>(
            Border.StrokeProperty,
            getter: static vm => vm.IsNextEnabled,
            convert: static enabled => enabled ? Color.FromArgb("#4FA8F6") : Color.FromArgb("#E2E8F0"))
        .Bind<Border, FillInQuizViewModel, bool, double>(
            Border.StrokeThicknessProperty,
            getter: static vm => vm.IsNextEnabled,
            convert: static enabled => enabled ? 2.0 : 1.5);

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

                        // Keep progress ring locked to LTR even when the page is RTL
                        new CircularProgressView
                        {
                            FlowDirection = FlowDirection.LeftToRight
                        }
                        .CenterHorizontal()
                        .Bind(CircularProgressView.ProgressProperty, static (FillInQuizViewModel vm) => vm.ProgressValue)
                        .Bind(CircularProgressView.TextProperty, static (FillInQuizViewModel vm) => vm.ProgressText),

                        // Formatted Sentence with Inline Blank (flows RTL automatically in Hebrew)
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
                                stringFormat: AppResources.FillInHintFormat)
                        }.Start()
                    }
                }.Row(1).Top(),

                // Bottom "Next" / "הבא" Pill Button
                new QuizNextButton
                {
                    Text = AppResources.NextButtonText,
                    Command = viewModel.NextCommand
                }
                .Row(2)
                .Bind(IsEnabledProperty, static (FillInQuizViewModel vm) => vm.IsNextEnabled)
            }
        };
    }

    private static AnalyzingOverlayView BuildAnalyzingOverlay() =>
        new AnalyzingOverlayView()
            .Bind(IsVisibleProperty, static (FillInQuizViewModel vm) => vm.IsAnalyzing);

    private static QuizSummaryView BuildSummaryView(FillInQuizViewModel viewModel) =>
        new QuizSummaryView
        {
            Title = AppResources.FillInResultsTitle,
            RetakeButtonText = AppResources.RetakeQuizText,
            RetakeCommand = viewModel.RestartCommand,
            GoHomeCommand = viewModel.GoHomeCommand,
            ItemTemplate = new DataTemplate(CreateReviewItemCard)
        }
        .Bind(IsVisibleProperty, static (FillInQuizViewModel vm) => vm.IsSummaryVisible)
        .Bind(QuizSummaryView.SubtitleProperty, static (FillInQuizViewModel vm) => vm.ScoreSummaryText)
        .Bind(QuizSummaryView.ItemsSourceProperty, static (FillInQuizViewModel vm) => vm.ReviewResults);

    private static Grid CreateReviewItemCard() =>
        new()
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
                                stringFormat: AppResources.FillInYourAnswerFormat),

                            new Label
                            {
                                TextColor = Color.FromArgb("#16A34A"),
                                FontSize = 13
                            }
                            .Bind(
                                Label.TextProperty,
                                static (FillInReviewItem item) => item.CorrectAnswer,
                                stringFormat: AppResources.FillInExpectedFormat)
                            .Bind(
                                IsVisibleProperty,
                                static (FillInReviewItem item) => item.IsCorrect,
                                convert: static isCorrect => !isCorrect)
                        }
                    }
                }
            }
        };
}