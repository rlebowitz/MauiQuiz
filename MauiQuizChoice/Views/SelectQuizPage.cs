using CommunityToolkit.Maui.Markup;
using MauiQuizChoice.Controls;
using MauiQuizChoice.Models;
using MauiQuizChoice.ViewModels;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace MauiQuizChoice.Views;

public class SelectQuizPage : ContentPage
{
    public SelectQuizPage()
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
                // Curved Purple Header
                new Border
                {
                    BackgroundColor = Color.FromArgb("#6B66F6"),
                    StrokeThickness = 0,
                    Margin = new Thickness(-40, -50, -40, 0),
                    StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(0, 0, 180, 180) }
                }.Row(0),

                // Header Navigation + Progress + Question
                new VerticalStackLayout
                {
                    Padding = new Thickness(24, 16, 24, 30),
                    Spacing = 14,
                    Children =
                    {
                        // Top Navigation Bar
                        new Grid
                        {
                            ColumnDefinitions = Columns.Define(40, Star, 40),
                            HeightRequest = 36,
                            Children =
                            {
                                // Back Arrow (Hidden on Question 1)
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
                                .Bind(IsVisibleProperty, static (SelectViewModel vm) => vm.CanGoBack),

                                // Close / Reset Button
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
                nextButton
            }
        };
    }

    private static View CreateOptionCard(SelectViewModel viewModel)
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
            .Bind(IsVisibleProperty, static (AnswerOption opt) => opt.IsSelected)
        }
        .Column(1)
        .Size(22);

        radioCircle.Triggers.Add(new DataTrigger(typeof(Border))
        {
            Binding = new Binding(nameof(AnswerOption.IsSelected)),
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
                Children =
                {
                    new Label
                    {
                        TextColor = Color.FromArgb("#2D3748"),
                        FontSize = 15
                    }
                    .Column(0)
                    .CenterVertical()
                    .Bind(Label.TextProperty, static (AnswerOption opt) => opt.Text),

                    radioCircle
                }
            }
        };

        card.Triggers.Add(new DataTrigger(typeof(Border))
        {
            Binding = new Binding(nameof(AnswerOption.IsSelected)),
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
        }.Bind(IsVisibleProperty, static (SelectViewModel vm) => vm.IsAnalyzing);

    private static Grid BuildSummaryView(SelectViewModel viewModel) =>
        new Grid
        {
            RowDefinitions = Rows.Define(200, Star, Auto),
            BackgroundColor = Color.FromArgb("#F8F9FB"),
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
                            Text = "Assessment Summary",
                            TextColor = Colors.White,
                            FontSize = 20,
                            FontAttributes = FontAttributes.Bold
                        }.TextCenterHorizontal(),

                        new Label
                        {
                            Text = "Here is a review of your responses",
                            TextColor = Color.FromArgb("#E0E7FF"),
                            FontSize = 14
                        }.TextCenterHorizontal()
                    }
                }.Row(0).CenterVertical(),

                // Summary Response Cards
                new CollectionView
                {
                    Margin = new Thickness(24, 12, 24, 0),
                    SelectionMode = SelectionMode.None,
                    ItemTemplate = new DataTemplate(CreateSummaryItemCard)
                }
                .Row(1)
                .Bind(ItemsView.ItemsSourceProperty, static (SelectViewModel vm) => vm.ReviewResults),

                // Bottom "Retake Quiz" Pill Button
                new Button
                {
                    Text = "Retake Assessment",
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
        }.Bind(IsVisibleProperty, static (SelectViewModel vm) => vm.IsSummaryVisible);

    private static View CreateSummaryItemCard() =>
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