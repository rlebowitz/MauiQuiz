

using MauiQuizChoice.Views;
namespace MauiQuizChoice
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(SelectQuizPage), typeof(Views.SelectQuizPage));
            Routing.RegisterRoute(nameof(MatchingQuizPage), typeof(MatchingQuizPage));
            Routing.RegisterRoute(nameof(TrueFalseQuizPage), typeof(TrueFalseQuizPage));
            Routing.RegisterRoute(nameof(MultiSelectQuizPage), typeof(MultiSelectQuizPage));
            Routing.RegisterRoute(nameof(FillInQuizPage), typeof(FillInQuizPage));
        }
    }
}
