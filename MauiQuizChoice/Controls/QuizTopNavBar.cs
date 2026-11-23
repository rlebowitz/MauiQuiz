using System.Windows.Input;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace MauiQuizChoice.Controls;

public partial class QuizTopNavBar : Grid
{
    [BindableProperty]
    public partial ICommand? PreviousCommand { get; set; }
    [BindableProperty]
    public partial ICommand? GoHomeCommand { get; set; }
    [BindableProperty]
    public partial bool CanGoBack { get; set; } = false;

    public QuizTopNavBar()
    {
        ColumnDefinitions = Columns.Define(40, Star, 40);
        HeightRequest = 36;

        var backTap = new TapGestureRecognizer();
        backTap.SetBinding(TapGestureRecognizer.CommandProperty, new Binding(nameof(PreviousCommand), source: this));

        var backButton = new Border
        {
            BackgroundColor = Colors.Transparent,
            StrokeThickness = 0,
            GestureRecognizers = { backTap },
            Content = new Label
            {
                Text = "←",
                TextColor = Colors.White,
                FontSize = 22
            }.Start().CenterVertical()
        }.Column(0);
        backButton.SetBinding(IsVisibleProperty, new Binding(nameof(CanGoBack), source: this));

        var homeTap = new TapGestureRecognizer();
        homeTap.SetBinding(TapGestureRecognizer.CommandProperty, new Binding(nameof(GoHomeCommand), source: this));

        var homeButton = new Border
        {
            BackgroundColor = Colors.Transparent,
            StrokeThickness = 0,
            GestureRecognizers = { homeTap },
            Content = new Label
            {
                Text = "✕",
                TextColor = Colors.White,
                FontSize = 16
            }.End().CenterVertical()
        }.Column(2);

        Children.Add(backButton);
        Children.Add(homeButton);
    }
}