using System.Linq.Expressions;
using CommunityToolkit.Maui.Markup;
using MauiQuizChoice.Controls;
using MauiQuizChoice.Models;
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

                new AnalyzingOverlayView()
                    .Bind(IsVisibleProperty, static (TrueFalseQuizViewModel vm) => vm.IsAnalyzing),

                new QuizSummaryView
                {
                    Title = "True / False Results",
                    RetakeButtonText = "Retake Quiz",
                    RetakeCommand = viewModel.RestartCommand,
                    GoHomeCommand = viewModel.GoHomeCommand,
                    ItemTemplate = new DataTemplate(CreateReviewItemCard)
                }
                .Bind(IsVisibleProperty, static (TrueFalseQuizViewModel vm) => vm.IsSummaryVisible)
                .Bind(QuizSummaryView.SubtitleProperty, static (TrueFalseQuizViewModel vm) => vm.ScoreSummaryText)
                .Bind(QuizSummaryView.ItemsSourceProperty, static (TrueFalseQuizViewModel vm) => vm.ReviewResults)
            }
        };
    }

    private static Grid BuildMainQuizView(TrueFalseQuizViewModel viewModel) =>
        new Grid
        {
            RowDefinitions = Rows.Define(260, Star, Auto),
            Children =
            {
                new CurvedHeaderBackground().Row(0),

                // Header Navigation + Progress + Statement
                new VerticalStackLayout
                {
                    Padding = new Thickness(24, 16, 24, 24),
                    Spacing = 14,
                    Children =
                    {
                        new QuizTopNavBar
                        {
                            PreviousCommand = viewModel.PreviousCommand,
                            GoHomeCommand = viewModel.GoHomeCommand
                        }.Bind(QuizTopNavBar.CanGoBackProperty, static (TrueFalseQuizViewModel vm) => vm.CanGoBack),

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
                        .Bind(Label.TextProperty, static (TrueFalseQuizViewModel vm) => vm.CurrentItem.Statement)
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
                            choiceValue: true,
                            isSelectedSelector: static vm => vm.CurrentItem.IsTrueSelected
                        ).Column(0),

                        CreateChoiceCard(
                            viewModel: viewModel,
                            labelText: "False",
                            glyph: "✕",
                            glyphSize: 20,
                            choiceValue: false,
                            isSelectedSelector: static vm => vm.CurrentItem.IsFalseSelected
                        ).Column(1)
                    }
                }.Row(1).CenterVertical(),

                // Bottom "Next" Pill Button
                new QuizNextButton { Command = viewModel.NextCommand }
                    .Row(2)
                    .Bind(IsEnabledProperty, static (TrueFalseQuizViewModel vm) => vm.IsNextEnabled)
            }
        };

    private static Border CreateChoiceCard(
     TrueFalseQuizViewModel viewModel,
     string labelText,
     string glyph,
     double glyphSize,
     bool choiceValue,
     Expression<Func<TrueFalseQuizViewModel, bool>> isSelectedSelector)
    {
        var iconLabel = new Label
        {
            Text = glyph,
            FontSize = glyphSize,
            FontAttributes = FontAttributes.Bold
        }
        .Center()
        .Bind<Label, TrueFalseQuizViewModel, bool, Color>(
            Label.TextColorProperty,
            getter: isSelectedSelector,
            convert: static (bool selected) => selected ? Colors.White : Color.FromArgb("#64748B"));

        var iconCircle = new Border
        {
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 26 },
            Content = iconLabel
        }
        .Size(52)
        .CenterHorizontal()
        .Bind<Border, TrueFalseQuizViewModel, bool, Color>(
            VisualElement.BackgroundColorProperty,
            getter: isSelectedSelector,
            convert: static (bool selected) => selected ? Color.FromArgb("#4FA8F6") : Color.FromArgb("#F1F5F9"));

        return new Border
        {
            HeightRequest = 170,
            Padding = 16,
            StrokeShape = new RoundRectangle { CornerRadius = 16 },
            GestureRecognizers =
        {
            new TapGestureRecognizer
            {
                Command = viewModel.SelectAnswerCommand,
                CommandParameter = choiceValue
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
        }
        .Bind<Border, TrueFalseQuizViewModel, bool, Brush>(
            Border.StrokeProperty,
            getter: isSelectedSelector,
            convert: static (bool selected) => selected ? Color.FromArgb("#4FA8F6") : Color.FromArgb("#E2E8F0"))
        .Bind<Border, TrueFalseQuizViewModel, bool, double>(
            Border.StrokeThicknessProperty,
            getter: isSelectedSelector,
            convert: static (bool selected) => selected ? 2.5 : 1.5)
        .Bind<Border, TrueFalseQuizViewModel, bool, Color>(
            VisualElement.BackgroundColorProperty,
            getter: isSelectedSelector,
            convert: static (bool selected) => selected ? Color.FromArgb("#EFF6FF") : Colors.White);
    }

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