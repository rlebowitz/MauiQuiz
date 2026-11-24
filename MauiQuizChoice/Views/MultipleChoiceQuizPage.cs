using CommunityToolkit.Maui.Markup;
using MauiQuizChoice.Controls;
using MauiQuizChoice.Models;
using MauiQuizChoice.ViewModels;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace MauiQuizChoice.Views;

public class MultipleChoiceQuizPage : ContentPage
{
    public MultipleChoiceQuizPage()
    {
        var viewModel = new SelectViewModel();
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

    private static Grid BuildMainQuizView(SelectViewModel viewModel)
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
            Binding = new Binding(nameof(SelectViewModel.IsNextEnabled)),
            Value = true,
            Setters =
            {
                new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromArgb("#4FA8F6") }
            }
        });

        return new Grid
        {
            RowDefinitions = Rows.Define(250, Star, Auto),
            Children =
            {
                new CurvedHeaderBackground().Row(0),

                // Header Navigation + Progress + Question
                new VerticalStackLayout
                {
                    Padding = new Thickness(24, 16, 24, 30),
                    Spacing = 14,
                    Children =
                    {
                        // Top Navigation Bar
                        new QuizTopNavBar
                        {
                            PreviousCommand = viewModel.PreviousCommand,
                            GoHomeCommand = viewModel.GoHomeCommand
                        }.Bind(QuizTopNavBar.CanGoBackProperty, static (SelectViewModel vm) => vm.CanGoBack),

                        // Animated Circular Progress Indicator
                        new CircularProgressView()
                            .CenterHorizontal()
                            .Bind(CircularProgressView.ProgressProperty, static (SelectViewModel vm) => vm.ProgressValue)
                            .Bind(CircularProgressView.TextProperty, static (SelectViewModel vm) => vm.ProgressText),

                        // Question Text
                        new Label
                        {
                            TextColor = Colors.White,
                            FontSize = 18,
                            FontAttributes = FontAttributes.Bold,
                            LineBreakMode = LineBreakMode.WordWrap
                        }
                        .TextCenterHorizontal()
                        .Bind(Label.TextProperty, static (SelectViewModel vm) => vm.CurrentItem.Text)
                    }
                }.Row(0),

                // Answer Options List
                new CollectionView
                {
                    Margin = new Thickness(24, 10, 24, 0),
                    SelectionMode = SelectionMode.None,
                    ItemTemplate = new DataTemplate(() => CreateOptionCard(viewModel))
                }
                .Row(1)
                .Bind(ItemsView.ItemsSourceProperty, static (SelectViewModel vm) => vm.CurrentItem.Options),

                // Bottom "Next" Pill Button
               new QuizNextButton{Command = viewModel.NextCommand}.Row(2).Bind(IsEnabledProperty, static (SelectViewModel vm) => vm.IsNextEnabled)
            }
        };
    }

    private static Grid CreateOptionCard(SelectViewModel viewModel)
    {
        var tapGesture = new TapGestureRecognizer
        {
            Command = viewModel.SelectOptionCommand
        };
        tapGesture.Bind(TapGestureRecognizer.CommandParameterProperty, ".");

        var radioCircle = new Border
        {
            Stroke = Color.FromArgb("#4A5568"),
            StrokeThickness = 1.5,
            BackgroundColor = Colors.Transparent,
            StrokeShape = new RoundRectangle { CornerRadius = 11 },
            Content = new Label
            {
                Text = "✓",
                TextColor = Colors.White,
                FontSize = 12,
                FontAttributes = FontAttributes.Bold
            }
            .Center()
            .Bind(IsVisibleProperty, static (MultipleChoiceAnswerOption opt) => opt.IsSelected)
        }
        .Column(1)
        .Size(22);

        radioCircle.Triggers.Add(new DataTrigger(typeof(Border))
        {
            Binding = new Binding(nameof(MultipleChoiceAnswerOption.IsSelected)),
            Value = true,
            Setters =
            {
                new Setter { Property = BackgroundColorProperty, Value = Color.FromArgb("#4FA8F6") },
                new Setter { Property = Border.StrokeProperty, Value = Color.FromArgb("#4FA8F6") }
            }
        });

        var card = new Border
        {
            BackgroundColor = Colors.White,
            Stroke = Color.FromArgb("#E2E8F0"),
            StrokeThickness = 1.5,
            Padding = new Thickness(18, 16),
            StrokeShape = new RoundRectangle { CornerRadius = 10 },
            GestureRecognizers = { tapGesture },
            Content = new Grid
            {
                ColumnDefinitions = Columns.Define(Star, Auto),
                Children =
                {
                    new Label
                    {
                        TextColor = Color.FromArgb("#2D3748"),
                        FontSize = 15
                    }
                    .Column(0)
                    .CenterVertical()
                    .Bind(Label.TextProperty, static (MultipleChoiceAnswerOption opt) => opt.Text),

                    radioCircle
                }
            }
        };

        card.Triggers.Add(new DataTrigger(typeof(Border))
        {
            Binding = new Binding(nameof(MultipleChoiceAnswerOption.IsSelected)),
            Value = true,
            Setters =
            {
                new Setter { Property = Border.StrokeProperty, Value = Color.FromArgb("#4FA8F6") },
                new Setter { Property = Border.StrokeThicknessProperty, Value = 2.0 }
            }
        });

        return new Grid
        {
            Padding = new Thickness(0, 6),
            Children = { card }
        };
    }

    private static AnalyzingOverlayView BuildAnalyzingOverlay() =>
        new AnalyzingOverlayView().Bind(IsVisibleProperty, static (SelectViewModel vm) => vm.IsAnalyzing);

    private static QuizSummaryView BuildSummaryView(SelectViewModel viewModel) =>
       new QuizSummaryView
       {
           Title = "Multiple Choice Results",
           RetakeButtonText = "Retake Quiz",
           RetakeCommand = viewModel.RestartCommand,
           GoHomeCommand = viewModel.GoHomeCommand,
           ItemTemplate = new DataTemplate(CreateSummaryItemCard)

       }
       .Bind(IsVisibleProperty, static (SelectViewModel vm) => vm.IsSummaryVisible)
        .Bind(QuizSummaryView.SubtitleProperty, static (SelectViewModel vm) => vm.ScoreSummaryText)
        .Bind(QuizSummaryView.ItemsSourceProperty, static (SelectViewModel vm) => vm.ReviewResults);


    private static Grid CreateSummaryItemCard() =>
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
                        Spacing = 6,
                        Children =
                        {
                            new Label
                            {
                                TextColor = Color.FromArgb("#4A5568"),
                                FontSize = 13
                            }.Bind(Label.TextProperty, static (SummaryItem item) => item.QuestionText),

                            new Label
                            {
                                TextColor = Color.FromArgb("#6B66F6"),
                                FontSize = 15,
                                FontAttributes = FontAttributes.Bold
                            }.Bind(Label.TextProperty, static (SummaryItem item) => item.SelectedAnswer)
                        }
                    }
                }
            }
        };
}