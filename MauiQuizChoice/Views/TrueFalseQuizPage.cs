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
                new Setter { Property = BackgroundColorProperty, Value = Color.FromArgb("#4FA8F6") }
            }
        });

        return new Grid
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
                            commandParameter: "True",
                            selectedBindingPath: $"{nameof(TrueFalseQuizViewModel.CurrentItem)}.IsTrueSelected"
                        ).Column(0),

                        CreateChoiceCard(
                            viewModel: viewModel,
                            labelText: "False",
                            glyph: "✕",
                            glyphSize: 20,
                            commandParameter: "False",
                            selectedBindingPath: $"{nameof(TrueFalseQuizViewModel.CurrentItem)}.IsFalseSelected"
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
                new Setter { Property = BackgroundColorProperty, Value = Color.FromArgb("#4FA8F6") }
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
            //Binding = new Binding(selectedBindingPath),
            //Value = true,
            Binding = new Binding($"{nameof(TrueFalseQuizViewModel.CurrentItem)}.SelectedAnswer"),
            Value = labelText == "True", // `true` for True card, `false` for False card
            Setters =
            {
                new Setter { Property = Border.StrokeProperty, Value = Color.FromArgb("#4FA8F6") },
                new Setter { Property = Border.StrokeThicknessProperty, Value = 2.5 },
                new Setter { Property = BackgroundColorProperty, Value = Color.FromArgb("#EFF6FF") }
            }
        });

        return card;
    }

    private static AnalyzingOverlayView BuildAnalyzingOverlay() =>
        new AnalyzingOverlayView()
            .Bind(IsVisibleProperty, static (TrueFalseQuizViewModel vm) => vm.IsAnalyzing);

    private static QuizSummaryView BuildSummaryView(TrueFalseQuizViewModel viewModel) =>
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
        .Bind(QuizSummaryView.ItemsSourceProperty, static (TrueFalseQuizViewModel vm) => vm.ReviewResults);

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