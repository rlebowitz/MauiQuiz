using CommunityToolkit.Maui.Markup;
using MauiQuizChoice.Controls;
using MauiQuizChoice.Models;
using MauiQuizChoice.ViewModels;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace MauiQuizChoice.Views;

public class MultiSelectQuizPage : ContentPage
{
    public MultiSelectQuizPage()
    {
        var viewModel = new MultiSelectQuizViewModel();
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

    private static Grid BuildMainQuizView(MultiSelectQuizViewModel viewModel)
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
            Binding = new Binding(nameof(MultiSelectQuizViewModel.IsNextEnabled)),
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

                // Header Navigation + Progress + Hint Pill + Question
                new VerticalStackLayout
                {
                    Padding = new Thickness(24, 14, 24, 24),
                    Spacing = 10,
                    Children =
                    {
                        new QuizTopNavBar
                        {
                            PreviousCommand = viewModel.PreviousCommand,
                            GoHomeCommand = viewModel.GoHomeCommand
                        }.Bind(QuizTopNavBar.CanGoBackProperty, static (MultiSelectQuizViewModel vm) => vm.CanGoBack),

                        new CircularProgressView()
                            .CenterHorizontal()
                            .Bind(CircularProgressView.ProgressProperty, static (MultiSelectQuizViewModel vm) => vm.ProgressValue)
                            .Bind(CircularProgressView.TextProperty, static (MultiSelectQuizViewModel vm) => vm.ProgressText),

                        // Multi-select Hint Badge
                        new Border
                        {
                            BackgroundColor = Color.FromArgb("#5752D8"),
                            StrokeThickness = 0,
                            Padding = new Thickness(12, 4),
                            StrokeShape = new RoundRectangle { CornerRadius = 12 },
                            Content = new Label
                            {
                                Text = "SELECT ALL THAT APPLY",
                                TextColor = Color.FromArgb("#E0E7FF"),
                                FontSize = 10,
                                FontAttributes = FontAttributes.Bold,
                                CharacterSpacing = 0.8
                            }
                        }.CenterHorizontal(),

                        new Label
                        {
                            TextColor = Colors.White,
                            FontSize = 17,
                            FontAttributes = FontAttributes.Bold,
                            LineBreakMode = LineBreakMode.WordWrap
                        }
                        .TextCenterHorizontal()
                        .Bind(Label.TextProperty, static (MultiSelectQuizViewModel vm) => vm.CurrentItem.Text)
                    }
                }.Row(0),

                // Multi-Select Checkbox Cards
                new CollectionView
                {
                    Margin = new Thickness(24, 10, 24, 0),
                    SelectionMode = SelectionMode.None,
                    ItemTemplate = new DataTemplate(() => CreateOptionCard(viewModel))
                }
                .Row(1)
                .Bind(ItemsView.ItemsSourceProperty, static (MultiSelectQuizViewModel vm) => vm.CurrentItem.Options),

                // Bottom "Next" Pill Button
                nextButton
            }
        };
    }

    private static Grid CreateOptionCard(MultiSelectQuizViewModel viewModel)
    {
        var tapGesture = new TapGestureRecognizer
        {
            Command = viewModel.ToggleOptionCommand
        };
        tapGesture.Bind(TapGestureRecognizer.CommandParameterProperty, ".");

        var checkbox = new Border
        {
            Stroke = Color.FromArgb("#4A5568"),
            StrokeThickness = 1.5,
            BackgroundColor = Colors.Transparent,
            StrokeShape = new RoundRectangle { CornerRadius = 6 },
            Content = new Label
            {
                Text = "✓",
                TextColor = Colors.White,
                FontSize = 12,
                FontAttributes = FontAttributes.Bold
            }
            .Center()
            .Bind(IsVisibleProperty, static (MultiSelectOption opt) => opt.IsSelected)
        }
        .Column(1)
        .Size(22);

        checkbox.Triggers.Add(new DataTrigger(typeof(Border))
        {
            Binding = new Binding(nameof(MultiSelectOption.IsSelected)),
            Value = true,
            Setters =
            {
                new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromArgb("#4FA8F6") },
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
                ColumnSpacing = 12,
                Children =
                {
                    new Label
                    {
                        TextColor = Color.FromArgb("#2D3748"),
                        FontSize = 15
                    }
                    .Column(0)
                    .CenterVertical()
                    .Bind(Label.TextProperty, static (MultiSelectOption opt) => opt.Text),

                    checkbox
                }
            }
        };

        card.Triggers.Add(new DataTrigger(typeof(Border))
        {
            Binding = new Binding(nameof(MultiSelectOption.IsSelected)),
            Value = true,
            Setters =
            {
                new Setter { Property = Border.StrokeProperty, Value = Color.FromArgb("#4FA8F6") },
                new Setter { Property = Border.StrokeThicknessProperty, Value = 2.0 },
                new Setter { Property = BackgroundColorProperty, Value = Color.FromArgb("#F8FAFC") }
            }
        });

        return new Grid
        {
            Padding = new Thickness(0, 6),
            Children = { card }
        };
    }

    private static AnalyzingOverlayView BuildAnalyzingOverlay() =>
        new AnalyzingOverlayView()
            .Bind(IsVisibleProperty, static (MultiSelectQuizViewModel vm) => vm.IsAnalyzing);

    private static QuizSummaryView BuildSummaryView(MultiSelectQuizViewModel viewModel) =>
        new QuizSummaryView
        {
            Title = "Multi-Select Results",
            RetakeButtonText = "Retake Quiz",
            RetakeCommand = viewModel.RestartCommand,
            GoHomeCommand = viewModel.GoHomeCommand,
            ItemTemplate = new DataTemplate(CreateReviewItemCard)
        }
        .Bind(IsVisibleProperty, static (MultiSelectQuizViewModel vm) => vm.IsSummaryVisible)
        .Bind(QuizSummaryView.SubtitleProperty, static (MultiSelectQuizViewModel vm) => vm.ScoreSummaryText)
        .Bind(QuizSummaryView.ItemsSourceProperty, static (MultiSelectQuizViewModel vm) => vm.ReviewResults);

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
                            }.Bind(Label.TextProperty, static (MultiSelectReviewItem item) => item.QuestionText),

                            new Label
                            {
                                TextColor = Color.FromArgb("#6B66F6"),
                                FontSize = 13
                            }.Bind(
                                Label.TextProperty,
                                static (MultiSelectReviewItem item) => item.UserSelectionsText,
                                stringFormat: "Selected: {0}"),

                            new Label
                            {
                                TextColor = Color.FromArgb("#16A34A"),
                                FontSize = 13
                            }
                            .Bind(
                                Label.TextProperty,
                                static (MultiSelectReviewItem item) => item.CorrectSelectionsText,
                                stringFormat: "Correct set: {0}")
                            .Bind(
                                IsVisibleProperty,
                                static (MultiSelectReviewItem item) => item.IsExactMatch,
                                convert: static (bool isExactMatch) => !isExactMatch)
                        }
                    }
                }
            }
        };
}