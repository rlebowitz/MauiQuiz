using CommunityToolkit.Maui.Markup;
using MauiQuizChoice.Controls;
using MauiQuizChoice.Models;
using MauiQuizChoice.ViewModels;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace MauiQuizChoice.Views;

public class MatchingQuizPage : ContentPage
{
    public MatchingQuizPage()
    {
        var viewModel = new MatchingQuizViewModel();
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

    private static Grid BuildMainQuizView(MatchingQuizViewModel viewModel)
    {
        var promptsStack = new VerticalStackLayout { Spacing = 12 }.Column(0);
        BindableLayout.SetItemTemplate(promptsStack, new DataTemplate(() => CreatePromptCard(viewModel)));
        promptsStack.Bind(BindableLayout.ItemsSourceProperty, static (MatchingQuizViewModel vm) => vm.CurrentRound.Prompts);

        var targetsStack = new VerticalStackLayout { Spacing = 12 }.Column(1);
        BindableLayout.SetItemTemplate(targetsStack, new DataTemplate(() => CreateTargetCard(viewModel)));
        targetsStack.Bind(BindableLayout.ItemsSourceProperty, static (MatchingQuizViewModel vm) => vm.CurrentRound.Targets);

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
            Margin = new Thickness(24, 10, 24, 32)
        }.Row(2);

        nextButton.Triggers.Add(new DataTrigger(typeof(Button))
        {
            Binding = new Binding(nameof(MatchingQuizViewModel.IsNextEnabled)),
            Value = true,
            Setters =
            {
                new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromArgb("#4FA8F6") }
            }
        });

        return new Grid
        {
            RowDefinitions = Rows.Define(240, Star, Auto),
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

                // Header Bar + Circular Progress + Instruction
                new VerticalStackLayout
                {
                    Padding = new Thickness(24, 16, 24, 24),
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
                                .Bind(IsVisibleProperty, static (MatchingQuizViewModel vm) => vm.CanGoBack),

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
                            .Bind(CircularProgressView.ProgressProperty, static (MatchingQuizViewModel vm) => vm.ProgressValue)
                            .Bind(CircularProgressView.TextProperty, static (MatchingQuizViewModel vm) => vm.ProgressText),

                        new Label
                        {
                            TextColor = Colors.White,
                            FontSize = 17,
                            FontAttributes = FontAttributes.Bold,
                            LineBreakMode = LineBreakMode.WordWrap
                        }
                        .TextCenterHorizontal()
                        .Bind(Label.TextProperty, static (MatchingQuizViewModel vm) => vm.CurrentRound.Instruction)
                    }
                }.Row(0),

                // Two-Column Matching Area
                new ScrollView
                {
                    Margin = new Thickness(16, 8, 16, 0),
                    Content = new Grid
                    {
                        ColumnDefinitions = Columns.Define(Star, Star),
                        ColumnSpacing = 12,
                        Children = { promptsStack, targetsStack }
                    }.CenterVertical()
                }.Row(1),

                // Bottom "Next" Pill Button
                nextButton
            }
        };
    }

    private static View CreatePromptCard(MatchingQuizViewModel viewModel)
    {
        var tapGesture = new TapGestureRecognizer
        {
            Command = viewModel.SelectPromptCommand
        };
        tapGesture.Bind(TapGestureRecognizer.CommandParameterProperty, ".");

        var card = new Border
        {
            BackgroundColor = Colors.White,
            Stroke = Color.FromArgb("#E2E8F0"),
            StrokeThickness = 1.5,
            Padding = new Thickness(14, 16),
            MinimumHeightRequest = 88,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            GestureRecognizers = { tapGesture },
            Content = new Grid
            {
                RowDefinitions = Rows.Define(Auto, Star),
                RowSpacing = 6,
                Children =
                {
                    // Pair Number Badge
                    new Border
                    {
                        BackgroundColor = Color.FromArgb("#6B66F6"),
                        StrokeThickness = 0,
                        StrokeShape = new RoundRectangle { CornerRadius = 11 },
                        Content = new Label
                        {
                            TextColor = Colors.White,
                            FontSize = 11,
                            FontAttributes = FontAttributes.Bold
                        }
                        .Center()
                        .Bind(Label.TextProperty, static (MatchPromptItem item) => item.MatchedBadgeText)
                    }
                    .Row(0)
                    .Size(22)
                    .End()
                    .Bind(IsVisibleProperty, static (MatchPromptItem item) => item.IsMatched),

                    new Label
                    {
                        TextColor = Color.FromArgb("#2D3748"),
                        FontSize = 14,
                        FontAttributes = FontAttributes.Bold
                    }
                    .Row(1)
                    .CenterVertical()
                    .Bind(Label.TextProperty, static (MatchPromptItem item) => item.Text)
                }
            }
        };

        // Highlight Blue when actively selected
        card.Triggers.Add(new DataTrigger(typeof(Border))
        {
            Binding = new Binding(nameof(MatchPromptItem.IsActive)),
            Value = true,
            Setters =
            {
                new Setter { Property = Border.StrokeProperty, Value = Color.FromArgb("#4FA8F6") },
                new Setter { Property = Border.StrokeThicknessProperty, Value = 2.5 },
                new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.FromArgb("#EFF6FF") }
            }
        });

        // Highlight Purple when paired
        card.Triggers.Add(new DataTrigger(typeof(Border))
        {
            Binding = new Binding(nameof(MatchPromptItem.IsMatched)),
            Value = true,
            Setters =
            {
                new Setter { Property = Border.StrokeProperty, Value = Color.FromArgb("#6B66F6") },
                new Setter { Property = Border.StrokeThicknessProperty, Value = 2.0 }
            }
        });

        return card;
    }

    private static View CreateTargetCard(MatchingQuizViewModel viewModel)
    {
        var tapGesture = new TapGestureRecognizer
        {
            Command = viewModel.SelectTargetCommand
        };
        tapGesture.Bind(TapGestureRecognizer.CommandParameterProperty, ".");

        var card = new Border
        {
            BackgroundColor = Colors.White,
            Stroke = Color.FromArgb("#E2E8F0"),
            StrokeThickness = 1.5,
            Padding = new Thickness(14, 16),
            MinimumHeightRequest = 88,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            GestureRecognizers = { tapGesture },
            Content = new Grid
            {
                RowDefinitions = Rows.Define(Auto, Star),
                RowSpacing = 6,
                Children =
                {
                    // Matching Pair Number Badge
                    new Border
                    {
                        BackgroundColor = Color.FromArgb("#6B66F6"),
                        StrokeThickness = 0,
                        StrokeShape = new RoundRectangle { CornerRadius = 11 },
                        Content = new Label
                        {
                            TextColor = Colors.White,
                            FontSize = 11,
                            FontAttributes = FontAttributes.Bold
                        }
                        .Center()
                        .Bind(Label.TextProperty, static (MatchTargetItem item) => item.MatchedBadgeText)
                    }
                    .Row(0)
                    .Size(22)
                    .Start()
                    .Bind(IsVisibleProperty, static (MatchTargetItem item) => item.IsMatched),

                    new Label
                    {
                        TextColor = Color.FromArgb("#4A5568"),
                        FontSize = 13
                    }
                    .Row(1)
                    .CenterVertical()
                    .Bind(Label.TextProperty, static (MatchTargetItem item) => item.Text)
                }
            }
        };

        // Highlight Purple when paired
        card.Triggers.Add(new DataTrigger(typeof(Border))
        {
            Binding = new Binding(nameof(MatchTargetItem.IsMatched)),
            Value = true,
            Setters =
            {
                new Setter { Property = Border.StrokeProperty, Value = Color.FromArgb("#6B66F6") },
                new Setter { Property = Border.StrokeThicknessProperty, Value = 2.0 }
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
        }.Bind(IsVisibleProperty, static (MatchingQuizViewModel vm) => vm.IsAnalyzing);

    private static Grid BuildSummaryView(MatchingQuizViewModel viewModel) =>
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
                            Text = "Matching Results",
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
                        .Bind(Label.TextProperty, static (MatchingQuizViewModel vm) => vm.ScoreSummaryText)
                    }
                }.Row(0).CenterVertical(),

                new CollectionView
                {
                    Margin = new Thickness(24, 12, 24, 0),
                    SelectionMode = SelectionMode.None,
                    ItemTemplate = new DataTemplate(CreateReviewItemCard)
                }
                .Row(1)
                .Bind(ItemsView.ItemsSourceProperty, static (MatchingQuizViewModel vm) => vm.ReviewResults),

                new Button
                {
                    Text = "Try Again",
                    Command = viewModel.RestartCommand,
                    BackgroundColor = Color.FromArgb("#4FA8F6"),
                    TextColor = Colors.White,
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold,
                    HeightRequest = 52,
                    CornerRadius = 26,
                    Margin = new Thickness(24, 10, 24, 32)
                }.Row(2)
            }
        }.Bind(IsVisibleProperty, static (MatchingQuizViewModel vm) => vm.IsSummaryVisible);

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
                                FontSize = 15,
                                FontAttributes = FontAttributes.Bold
                            }.Bind(Label.TextProperty, static (MatchReviewItem item) => item.Prompt),

                            new Label
                            {
                                TextColor = Color.FromArgb("#6B66F6"),
                                FontSize = 13
                            }.Bind(
                                Label.TextProperty,
                                static (MatchReviewItem item) => item.UserMatch,
                                stringFormat: "Your match: {0}"),

                            new Label
                            {
                                TextColor = Color.FromArgb("#16A34A"),
                                FontSize = 13
                            }
                            .Bind(
                                Label.TextProperty,
                                static (MatchReviewItem item) => item.CorrectMatch,
                                stringFormat: "Correct: {0}")
                            .Bind(
                                IsVisibleProperty,
                                static (MatchReviewItem item) => item.IsCorrect,
                                convert: static (bool isCorrect) => !isCorrect)
                        }
                    }
                }
            }
        };
}