using System.Linq.Expressions;
using CommunityToolkit.Maui.Markup;
using MauiQuizChoice.Controls;
using MauiQuizChoice.Models;
using MauiQuizChoice.ViewModels;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace MauiQuizChoice.Views;

public class FlashcardQuizPage : ContentPage
{
    private bool _isFlipping;

    public FlashcardQuizPage()
    {
        var viewModel = new FlashcardQuizViewModel();
        BindingContext = viewModel;

        Shell.SetNavBarIsVisible(this, false);
        BackgroundColor = Color.FromArgb("#F8F9FB");

        Content = new Grid
        {
            Children =
            {
                BuildMainQuizView(viewModel),

                new AnalyzingOverlayView()
                    .Bind(IsVisibleProperty, static (FlashcardQuizViewModel vm) => vm.IsAnalyzing),

                new QuizSummaryView
                {
                    Title = "Flashcard Results",
                    RetakeButtonText = "Study Again",
                    RetakeCommand = viewModel.RestartCommand,
                    GoHomeCommand = viewModel.GoHomeCommand,
                    ItemTemplate = new DataTemplate(CreateReviewItemCard)
                }
                .Bind(IsVisibleProperty, static (FlashcardQuizViewModel vm) => vm.IsSummaryVisible)
                .Bind(QuizSummaryView.SubtitleProperty, static (FlashcardQuizViewModel vm) => vm.ScoreSummaryText)
                .Bind(QuizSummaryView.ItemsSourceProperty, static (FlashcardQuizViewModel vm) => vm.ReviewResults)
            }
        };
    }

    private Grid BuildMainQuizView(FlashcardQuizViewModel viewModel) =>
        new()
        {
            RowDefinitions = Rows.Define(220, Star, Auto),
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
                        }.Bind(QuizTopNavBar.CanGoBackProperty, static (FlashcardQuizViewModel vm) => vm.CanGoBack),

                        new CircularProgressView()
                            .CenterHorizontal()
                            .Bind(CircularProgressView.ProgressProperty, static (FlashcardQuizViewModel vm) => vm.ProgressValue)
                            .Bind(CircularProgressView.TextProperty, static (FlashcardQuizViewModel vm) => vm.ProgressText),

                        new Label
                        {
                            Text = "Tap the card to flip",
                            TextColor = Color.FromArgb("#E0E7FF"),
                            FontSize = 14
                        }.CenterHorizontal()
                    }
                }.Row(0),

                // Center Flip Card + Self-Assessment Buttons
                new VerticalStackLayout
                {
                    Padding = new Thickness(24, 0),
                    Spacing = 20,
                    Children =
                    {
                        BuildFlipCard(viewModel),

                        new Grid
                        {
                            ColumnDefinitions = Columns.Define(Star, Star),
                            ColumnSpacing = 14,
                            Children =
                            {
                                CreateAssessmentButton(
                                    viewModel: viewModel,
                                    text: "Needs Review",
                                    knownValue: false,
                                    activeColor: Color.FromArgb("#F59E0B"),
                                    isSelectedSelector: static vm => vm.CurrentItem.Known == false
                                ).Column(0),

                                CreateAssessmentButton(
                                    viewModel: viewModel,
                                    text: "Got It",
                                    knownValue: true,
                                    activeColor: Color.FromArgb("#16A34A"),
                                    isSelectedSelector: static vm => vm.CurrentItem.Known == true
                                ).Column(1)
                            }
                        }
                    }
                }.Row(1).CenterVertical(),

                new QuizNextButton { Command = viewModel.NextCommand }
                    .Row(2)
                    .Bind(IsEnabledProperty, static (FlashcardQuizViewModel vm) => vm.IsNextEnabled)
            }
        };

    private Border BuildFlipCard(FlashcardQuizViewModel viewModel)
    {
        var card = new Border
        {
            HeightRequest = 240,
            Padding = 24,
            StrokeThickness = 1.5,
            StrokeShape = new RoundRectangle { CornerRadius = 18 }
        }
        .Bind<Border, FlashcardQuizViewModel, bool, Color>(
            BackgroundColorProperty,
            getter: static vm => vm.CurrentItem.IsFlipped,
            convert: static (bool flipped) => flipped ? Color.FromArgb("#EFF6FF") : Colors.White)
        .Bind<Border, FlashcardQuizViewModel, bool, Brush>(
            Border.StrokeProperty,
            getter: static vm => vm.CurrentItem.IsFlipped,
            convert: static (bool flipped) => flipped ? Color.FromArgb("#4FA8F6") : Color.FromArgb("#E2E8F0"));

        var frontContent = new VerticalStackLayout
        {
            Spacing = 12,
            Children =
            {
                new Label
                {
                    Text = "QUESTION",
                    TextColor = Color.FromArgb("#6B66F6"),
                    FontSize = 11,
                    FontAttributes = FontAttributes.Bold,
                    CharacterSpacing = 1.2
                }.CenterHorizontal(),

                new Label
                {
                    TextColor = Color.FromArgb("#1E293B"),
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    LineBreakMode = LineBreakMode.WordWrap
                }
                .TextCenterHorizontal()
                .Bind(Label.TextProperty, static (FlashcardQuizViewModel vm) => vm.CurrentItem.FrontText)
            }
        }
        .Center()
        .Bind(
            IsVisibleProperty,
            static (FlashcardQuizViewModel vm) => vm.CurrentItem.IsFlipped,
            convert: static (bool flipped) => !flipped);

        var backContent = new VerticalStackLayout
        {
            Spacing = 12,
            Children =
            {
                new Label
                {
                    Text = "ANSWER",
                    TextColor = Color.FromArgb("#4FA8F6"),
                    FontSize = 11,
                    FontAttributes = FontAttributes.Bold,
                    CharacterSpacing = 1.2
                }.CenterHorizontal(),

                new Label
                {
                    TextColor = Color.FromArgb("#1E293B"),
                    FontSize = 17,
                    LineBreakMode = LineBreakMode.WordWrap
                }
                .TextCenterHorizontal()
                .Bind(Label.TextProperty, static (FlashcardQuizViewModel vm) => vm.CurrentItem.BackText)
            }
        }
        .Center()
        .Bind(IsVisibleProperty, static (FlashcardQuizViewModel vm) => vm.CurrentItem.IsFlipped);

        card.Content = new Grid { Children = { frontContent, backContent } };

        var tap = new TapGestureRecognizer();
        tap.Tapped += async (_, _) =>
        {
            if (_isFlipping || viewModel.CurrentItem is null)
                return;

            _isFlipping = true;
            await card.RotateYToAsync(90, 140, Easing.CubicIn);
            viewModel.CurrentItem.IsFlipped = !viewModel.CurrentItem.IsFlipped;
            card.RotationY = -90;
            await card.RotateYToAsync(0, 140, Easing.CubicOut);
            _isFlipping = false;
        };
        card.GestureRecognizers.Add(tap);

        return card;
    }

    private static Border CreateAssessmentButton(
        FlashcardQuizViewModel viewModel,
        string text,
        bool knownValue,
        Color activeColor,
        Expression<Func<FlashcardQuizViewModel, bool>> isSelectedSelector)
    {
        var label = new Label
        {
            Text = text,
            FontSize = 15,
            FontAttributes = FontAttributes.Bold
        }
        .Center()
        .Bind<Label, FlashcardQuizViewModel, bool, Color>(
            Label.TextColorProperty,
            getter: isSelectedSelector,
            convert: selected => selected ? Colors.White : Color.FromArgb("#334155"));

        return new Border
        {
            HeightRequest = 48,
            StrokeThickness = 1.5,
            StrokeShape = new RoundRectangle { CornerRadius = 24 },
            Content = label,
            GestureRecognizers =
            {
                new TapGestureRecognizer
                {
                    Command = viewModel.MarkCardCommand,
                    CommandParameter = knownValue
                }
            }
        }
        .Bind<Border, FlashcardQuizViewModel, bool, Color>(
            VisualElement.BackgroundColorProperty,
            getter: isSelectedSelector,
            convert: selected => selected ? activeColor : Colors.White)
        .Bind<Border, FlashcardQuizViewModel, bool, Brush>(
            Border.StrokeProperty,
            getter: isSelectedSelector,
            convert: selected => selected ? activeColor : Color.FromArgb("#CBD5E1"));
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
                            }.Bind(Label.TextProperty, static (FlashcardReviewItem item) => item.Prompt),

                            new Label
                            {
                                TextColor = Color.FromArgb("#475569"),
                                FontSize = 13
                            }.Bind(Label.TextProperty, static (FlashcardReviewItem item) => item.Answer),

                            new Label
                            {
                                FontSize = 12,
                                FontAttributes = FontAttributes.Bold
                            }
                            .Bind(Label.TextProperty, static (FlashcardReviewItem item) => item.StatusText)
                            .Bind<Label, FlashcardReviewItem, bool, Color>(
                                Label.TextColorProperty,
                                getter: static item => item.Known,
                                convert: static (bool known) => known ? Color.FromArgb("#16A34A") : Color.FromArgb("#D97706"))
                        }
                    }
                }
            }
        };
}