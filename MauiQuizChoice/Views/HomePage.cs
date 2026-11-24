using System.Globalization;
using CommunityToolkit.Maui.Markup;
using MauiQuizChoice.Controls;
using MauiQuizChoice.Resources.Strings;
using MauiQuizChoice.ViewModels;
using Microsoft.Maui.Controls.Shapes;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace MauiQuizChoice.Views;

public class HomePage : ContentPage
{
    public HomePage()
    {
        var viewModel = new HomeViewModel();
        BindingContext = viewModel;

        bool isRtl = CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;
        FlowDirection = isRtl ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

        Shell.SetNavBarIsVisible(this, false);
        BackgroundColor = Color.FromArgb("#F8F9FB");

        Content = new Grid
        {
            RowDefinitions = Rows.Define(230, Star),
            Children =
            {
                new CurvedHeaderBackground().Row(0),

                // Top-End Language Toggle Pill ("עברית" / "English")
                CreateLanguageTogglePill(isRtl)
                    .Row(0)
                    .Top()
                    .End()
                    .Margins(top: 16, left: 20, right: 20),

                // Header Branding & Greeting
                new VerticalStackLayout
                {
                    Spacing = 10,
                    Padding = new Thickness(28, 24, 28, 24),
                    Children =
                    {
                        new Border
                        {
                            BackgroundColor = Color.FromArgb("#5752D8"),
                            StrokeThickness = 0,
                            StrokeShape = new RoundRectangle { CornerRadius = 24 },
                            Content = new Label
                            {
                                Text = "?",
                                TextColor = Colors.White,
                                FontSize = 22,
                                FontAttributes = FontAttributes.Bold
                            }.Center()
                        }.Size(48).CenterHorizontal(),

                        new Label
                        {
                            Text = AppResources.HomeTitle,
                            TextColor = Colors.White,
                            FontSize = 24,
                            FontAttributes = FontAttributes.Bold
                        }.TextCenterHorizontal(),

                        new Label
                        {
                            Text = AppResources.HomeSubtitle,
                            TextColor = Color.FromArgb("#E0E7FF"),
                            FontSize = 14
                        }.TextCenterHorizontal()
                    }
                }.Row(0).CenterVertical(),

                // Quiz Mode Cards
                new CollectionView
                {
                    Margin = new Thickness(24, 14, 24, 24),
                    SelectionMode = SelectionMode.None,
                    ItemTemplate = new DataTemplate(() => CreateQuizModeCard(viewModel))
                }
                .Row(1)
                .Bind(ItemsView.ItemsSourceProperty, static (HomeViewModel vm) => vm.QuizModes)
            }
        };
    }

    private static Border CreateLanguageTogglePill(bool isCurrentlyHebrew)
    {
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (_, _) =>
        {
            string nextCulture = isCurrentlyHebrew ? "en" : "he";
            SetLanguage(nextCulture);
        };

        return new Border
        {
            BackgroundColor = Color.FromArgb("#5752D8"),
            Stroke = Color.FromArgb("#818CF8"),
            StrokeThickness = 1,
            Padding = new Thickness(12, 6),
            StrokeShape = new RoundRectangle { CornerRadius = 14 },
            GestureRecognizers = { tapGesture },
            Content = new HorizontalStackLayout
            {
                Spacing = 6,
                Children =
                {
                    new Label
                    {
                        Text = "🌐",
                        FontSize = 12
                    }.CenterVertical(),

                    new Label
                    {
                        Text = AppResources.LanguageToggleLabel,
                        TextColor = Colors.White,
                        FontSize = 12,
                        FontAttributes = FontAttributes.Bold
                    }.CenterVertical()
                }
            }
        };
    }

    private static void SetLanguage(string cultureCode)
    {
        var culture = new CultureInfo(cultureCode);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        if (Application.Current?.Windows.Count > 0)
        {
            Application.Current.Windows[0].Page = new AppShell();
        }
    }

    private static Grid CreateQuizModeCard(HomeViewModel viewModel)
    {
        var tapGesture = new TapGestureRecognizer { Command = viewModel.LaunchQuizCommand };
        tapGesture.Bind(TapGestureRecognizer.CommandParameterProperty, ".");

        return new Grid
        {
            Padding = new Thickness(0, 8),
            Children =
            {
                new Border
                {
                    BackgroundColor = Colors.White,
                    Stroke = Color.FromArgb("#E2E8F0"),
                    StrokeThickness = 1.5,
                    Padding = 18,
                    StrokeShape = new RoundRectangle { CornerRadius = 14 },
                    GestureRecognizers = { tapGesture },
                    Content = new Grid
                    {
                        ColumnDefinitions = Columns.Define(Auto, Star, Auto),
                        ColumnSpacing = 14,
                        Children =
                        {
                            // Left Mode Icon Badge
                            new Border
                            {
                                StrokeThickness = 0,
                                StrokeShape = new RoundRectangle { CornerRadius = 12 },
                                Content = new Label
                                {
                                    FontSize = 18,
                                    FontAttributes = FontAttributes.Bold
                                }
                                .Center()
                                .Bind(Label.TextProperty, static (QuizModeCard c) => c.IconGlyph)
                                .Bind(
                                    Label.TextColorProperty,
                                    static (QuizModeCard c) => c.IconTextHex,
                                    convert: static (string? hex) => string.IsNullOrWhiteSpace(hex) ? Colors.Transparent : Color.FromArgb(hex))
                            }
                            .Column(0)
                            .Size(48)
                            .CenterVertical()
                            .Bind(
                                BackgroundColorProperty,
                                static (QuizModeCard c) => c.IconBackgroundHex,
                                convert: static (string? hex) => string.IsNullOrWhiteSpace(hex) ? Colors.Transparent : Color.FromArgb(hex)),

                            // Title, Subtitle & Question Count Pill
                            new VerticalStackLayout
                            {
                                Spacing = 4,
                                Children =
                                {
                                    new Label
                                    {
                                        TextColor = Color.FromArgb("#2D3748"),
                                        FontSize = 16,
                                        FontAttributes = FontAttributes.Bold
                                    }.Bind(Label.TextProperty, static (QuizModeCard c) => c.Title),

                                    new Label
                                    {
                                        TextColor = Color.FromArgb("#64748B"),
                                        FontSize = 13,
                                        LineBreakMode = LineBreakMode.WordWrap
                                    }.Bind(Label.TextProperty, static (QuizModeCard c) => c.Subtitle),

                                    new Border
                                    {
                                        BackgroundColor = Color.FromArgb("#F1F5F9"),
                                        StrokeThickness = 0,
                                        Padding = new Thickness(8, 3),
                                        Margin = new Thickness(0, 4, 0, 0),
                                        StrokeShape = new RoundRectangle { CornerRadius = 6 },
                                        Content = new Label
                                        {
                                            TextColor = Color.FromArgb("#475569"),
                                            FontSize = 11,
                                            FontAttributes = FontAttributes.Bold
                                        }.Bind(Label.TextProperty, static (QuizModeCard c) => c.QuestionCountBadge)
                                    }.Start()
                                }
                            }.Column(1).CenterVertical(),

                            // Right Launch Chevron (mirrors automatically in RTL)
                            new Label
                            {
                                Text = isRtlChevron(),
                                TextColor = Color.FromArgb("#4FA8F6"),
                                FontSize = 26,
                                FontAttributes = FontAttributes.Bold
                            }.Column(2).CenterVertical()
                        }
                    }
                }
            }
        };
    }

    private static string isRtlChevron() =>
        CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft ? "‹" : "›";
}