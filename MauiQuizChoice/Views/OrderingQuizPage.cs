using CommunityToolkit.Maui.Markup;
using MauiQuizChoice.Controls;
using MauiQuizChoice.Models;
using MauiQuizChoice.ViewModels;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace MauiQuizChoice.Views;

public class OrderingQuizPage : ContentPage
{
    public OrderingQuizPage()
    {
        var viewModel = new OrderingQuizViewModel();
        BindingContext = viewModel;

        Shell.SetNavBarIsVisible(this, false);
        BackgroundColor = Color.FromArgb("#F8F9FB");

        Content = new Grid
        {
            Children =
            {
                BuildMainQuizView(viewModel),

                new AnalyzingOverlayView()
                    .Bind(IsVisibleProperty, static (OrderingQuizViewModel vm) => vm.IsAnalyzing),

                new QuizSummaryView
                {
                    Title = "Sequencing Results",
                    RetakeButtonText = "Retake Quiz",
                    RetakeCommand = viewModel.RestartCommand,
                    GoHomeCommand = viewModel.GoHomeCommand,
                    ItemTemplate = new DataTemplate(CreateReviewItemCard)
                }
                .Bind(IsVisibleProperty, static (OrderingQuizViewModel vm) => vm.IsSummaryVisible)
                .Bind(QuizSummaryView.SubtitleProperty, static (OrderingQuizViewModel vm) => vm.ScoreSummaryText)
                .Bind(QuizSummaryView.ItemsSourceProperty, static (OrderingQuizViewModel vm) => vm.ReviewResults)
            }
        };
    }

    private static Grid BuildMainQuizView(OrderingQuizViewModel viewModel)
    {
        var collectionView = new CollectionView
        {
            Margin = new Thickness(24, 10, 24, 0),
            SelectionMode = SelectionMode.None,
            CanReorderItems = true,
            ItemTemplate = new DataTemplate(() => CreateStepCard(viewModel))
        }
        .Row(1)
        .Bind(ItemsView.ItemsSourceProperty, static (OrderingQuizViewModel vm) => vm.CurrentItem.Steps);

        collectionView.ReorderCompleted += (_, _) => viewModel.OnDragReorderCompleted();

        return new Grid
        {
            RowDefinitions = Rows.Define(255, Star, Auto),
            Children =
            {
                new CurvedHeaderBackground().Row(0),

                new VerticalStackLayout
                {
                    Padding = new Thickness(24, 16, 24, 24),
                    Spacing = 12,
                    Children =
                    {
                        new QuizTopNavBar
                        {
                            PreviousCommand = viewModel.PreviousCommand,
                            GoHomeCommand = viewModel.GoHomeCommand
                        }.Bind(QuizTopNavBar.CanGoBackProperty, static (OrderingQuizViewModel vm) => vm.CanGoBack),

                        new CircularProgressView()
                            .CenterHorizontal()
                            .Bind(CircularProgressView.ProgressProperty, static (OrderingQuizViewModel vm) => vm.ProgressValue)
                            .Bind(CircularProgressView.TextProperty, static (OrderingQuizViewModel vm) => vm.ProgressText),

                        new Label
                        {
                            TextColor = Colors.White,
                            FontSize = 17,
                            FontAttributes = FontAttributes.Bold,
                            LineBreakMode = LineBreakMode.WordWrap
                        }
                        .TextCenterHorizontal()
                        .Bind(Label.TextProperty, static (OrderingQuizViewModel vm) => vm.CurrentItem.Prompt)
                    }
                }.Row(0),

                collectionView,

                new QuizNextButton { Command = viewModel.NextCommand }
                    .Row(2)
                    .Bind(IsEnabledProperty, static (OrderingQuizViewModel vm) => vm.IsNextEnabled)
            }
        };
    }

    private static Grid CreateStepCard(OrderingQuizViewModel viewModel)
    {
        var upTap = new TapGestureRecognizer { Command = viewModel.MoveUpCommand };
        upTap.Bind(TapGestureRecognizer.CommandParameterProperty, ".");

        var downTap = new TapGestureRecognizer { Command = viewModel.MoveDownCommand };
        downTap.Bind(TapGestureRecognizer.CommandParameterProperty, ".");

        return new Grid
        {
            Padding = new Thickness(0, 5),
            Children =
            {
                new Border
                {
                    BackgroundColor = Colors.White,
                    Stroke = Color.FromArgb("#E2E8F0"),
                    StrokeThickness = 1.5,
                    Padding = new Thickness(14, 12),
                    StrokeShape = new RoundRectangle { CornerRadius = 10 },
                    Content = new Grid
                    {
                        ColumnDefinitions = Columns.Define(30, Star, Auto),
                        ColumnSpacing = 12,
                        Children =
                        {
                            // Position Number Badge
                            new Border
                            {
                                BackgroundColor = Color.FromArgb("#EEF2FF"),
                                StrokeThickness = 0,
                                StrokeShape = new RoundRectangle { CornerRadius = 14 },
                                Content = new Label
                                {
                                    TextColor = Color.FromArgb("#6B66F6"),
                                    FontSize = 13,
                                    FontAttributes = FontAttributes.Bold
                                }
                                .Center()
                                .Bind(Label.TextProperty, static (OrderStepItem step) => step.DisplayNumber)
                            }.Column(0).Size(28).CenterVertical(),

                            // Step Text
                            new Label
                            {
                                TextColor = Color.FromArgb("#2D3748"),
                                FontSize = 14,
                                LineBreakMode = LineBreakMode.WordWrap
                            }
                            .Column(1)
                            .CenterVertical()
                            .Bind(Label.TextProperty, static (OrderStepItem step) => step.Text),

                            // Up / Down Reorder Buttons
                            new HorizontalStackLayout
                            {
                                Spacing = 6,
                                Children =
                                {
                                    new Border
                                    {
                                        BackgroundColor = Color.FromArgb("#F1F5F9"),
                                        StrokeThickness = 0,
                                        StrokeShape = new RoundRectangle { CornerRadius = 6 },
                                        GestureRecognizers = { upTap },
                                        Content = new Label
                                        {
                                            Text = "▲",
                                            FontSize = 11,
                                            TextColor = Color.FromArgb("#475569")
                                        }.Center()
                                    }.Size(30),

                                    new Border
                                    {
                                        BackgroundColor = Color.FromArgb("#F1F5F9"),
                                        StrokeThickness = 0,
                                        StrokeShape = new RoundRectangle { CornerRadius = 6 },
                                        GestureRecognizers = { downTap },
                                        Content = new Label
                                        {
                                            Text = "▼",
                                            FontSize = 11,
                                            TextColor = Color.FromArgb("#475569")
                                        }.Center()
                                    }.Size(30)
                                }
                            }.Column(2).CenterVertical()
                        }
                    }
                }
            }
        };
    }

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
                        Spacing = 6,
                        Children =
                        {
                            new Label
                            {
                                TextColor = Color.FromArgb("#2D3748"),
                                FontSize = 14,
                                FontAttributes = FontAttributes.Bold
                            }.Bind(Label.TextProperty, static (OrderingReviewItem item) => item.Prompt),

                            new Label
                            {
                                TextColor = Color.FromArgb("#6B66F6"),
                                FontSize = 13
                            }.Bind(
                                Label.TextProperty,
                                static (OrderingReviewItem item) => item.UserOrderText,
                                stringFormat: "Your order: {0}"),

                            new Label
                            {
                                TextColor = Color.FromArgb("#16A34A"),
                                FontSize = 13
                            }
                            .Bind(
                                Label.TextProperty,
                                static (OrderingReviewItem item) => item.CorrectOrderText,
                                stringFormat: "Correct order: {0}")
                            .Bind(
                                IsVisibleProperty,
                                static (OrderingReviewItem item) => item.IsCorrect,
                                convert: static (bool isCorrect) => !isCorrect)
                        }
                    }
                }
            }
        };
}