using System.Windows.Input;
using CommunityToolkit.Maui.Markup;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace MauiQuizChoice.Controls;

public class QuizTopNavBar : Grid
{
    public static readonly BindableProperty PreviousCommandProperty =
        BindableProperty.Create(nameof(PreviousCommand), typeof(ICommand), typeof(QuizTopNavBar));

    public static readonly BindableProperty GoHomeCommandProperty =
        BindableProperty.Create(nameof(GoHomeCommand), typeof(ICommand), typeof(QuizTopNavBar));

    public static readonly BindableProperty CanGoBackProperty =
        BindableProperty.Create(nameof(CanGoBack), typeof(bool), typeof(QuizTopNavBar), false);

    public ICommand? PreviousCommand
    {
        get => (ICommand?)GetValue(PreviousCommandProperty);
        set => SetValue(PreviousCommandProperty, value);
    }

    public ICommand? GoHomeCommand
    {
        get => (ICommand?)GetValue(GoHomeCommandProperty);
        set => SetValue(GoHomeCommandProperty, value);
    }

    public bool CanGoBack
    {
        get => (bool)GetValue(CanGoBackProperty);
        set => SetValue(CanGoBackProperty, value);
    }

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